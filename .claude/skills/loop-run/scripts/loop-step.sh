#!/usr/bin/env bash
# Deterministic keep-or-revert step for the Karpathy loop.
#   loop-step.sh --baseline              measure the untouched code, record it
#   loop-step.sh "<what you changed>"    judge the working-tree edit: keep or revert
# The LLM never decides keep/revert. This script does, from the metric alone.
set -uo pipefail

ROOT="$(git rev-parse --show-toplevel)" || exit 2
cd "$ROOT"
CFG=".karpathy/config.env"
[ -f "$CFG" ] || { echo "ERROR: $CFG missing. Run /loop-init first."; exit 2; }
# shellcheck disable=SC1090
source "$CFG"
: "${METRIC_NAME:?}" "${DIRECTION:?}" "${EVAL_CMD:?}" "${SCOPE:?}"
TIMEOUT_SEC="${TIMEOUT_SEC:-600}"
MIN_DELTA="${MIN_DELTA:-0}"
TARGET="${TARGET:-}"      # optional: stop signal when the metric reaches this value
GUARD_CMD="${GUARD_CMD:-true}"
RESULTS=".karpathy/results.tsv"
LOG=".karpathy/last_run.log"
[ -f "$RESULTS" ] || printf 'commit\tmetric\tstatus\tdescription\ttime\n' > "$RESULTS"

log() { printf '%s\t%s\t%s\t%s\t%s\n' "$1" "$2" "$3" "$4" "$(date -u +%FT%TZ)" >> "$RESULTS"; }

run_metric() {   # prints the number; rc 1 = eval crashed/timed out, rc 2 = no METRIC= line
  timeout "$TIMEOUT_SEC" bash -c "$EVAL_CMD" > "$LOG" 2>&1 || return 1
  local m
  m="$(grep -E '^METRIC=' "$LOG" | tail -1 | cut -d= -f2)"
  [[ "$m" =~ ^-?[0-9]+([.][0-9]+)?$ ]] || return 2
  echo "$m"
}

best_metric() {
  awk -F'\t' -v d="$DIRECTION" 'NR>1 && ($3=="keep"||$3=="baseline") {
    if (!s || (d=="lower" && $2<b) || (d=="higher" && $2>b)) { b=$2; s=1 } }
    END { if (s) print b }' "$RESULTS"
}

improved() {     # improved NEW BEST  -> exit 0 if NEW beats BEST by more than MIN_DELTA
  awk -v n="$1" -v b="$2" -v d="$DIRECTION" -v t="$MIN_DELTA" \
    'BEGIN { if (d=="lower") exit !(n < b - t); else exit !(n > b + t) }'
}

# ---------- baseline ----------
if [ "${1:-}" = "--baseline" ]; then
  [ -z "$(git status --porcelain)" ] || { echo "ERROR: working tree not clean. Commit or stash first."; exit 2; }
  timeout "$TIMEOUT_SEC" bash -c "$GUARD_CMD" > "$LOG" 2>&1 || { echo "ERROR: guard fails on untouched code:"; tail -n 20 "$LOG"; exit 2; }
  m="$(run_metric)"; rc=$?
  [ $rc -eq 0 ] || { echo "ERROR: baseline eval failed (rc=$rc). See $LOG"; tail -n 20 "$LOG"; exit 2; }
  log "$(git rev-parse --short HEAD)" "$m" baseline "baseline"
  echo "BASELINE $METRIC_NAME=$m"
  exit 0
fi

# ---------- one experiment ----------
desc="${1:-unnamed change}"
changed="$(git status --porcelain --untracked-files=all | sed -E 's/^.{3}//; s/.* -> //')"
if [ -z "$changed" ]; then log "-" "" no_change "$desc"; echo "NO_CHANGE: nothing was edited"; exit 0; fi

read -ra PATS <<< "$SCOPE"          # read -a: no filesystem glob expansion
bad=""
while IFS= read -r f; do
  ok=0
  case "$f" in .karpathy/*) ok=0 ;; *) for p in "${PATS[@]}"; do case "$f" in $p) ok=1 ;; esac; done ;; esac
  [ $ok -eq 1 ] || bad="$bad $f"
done <<< "$changed"
if [ -n "$bad" ]; then
  git reset -q --hard HEAD && git clean -qfd
  log "-" "" rejected_scope "$desc [touched:$bad]"
  echo "REVERT rejected_scope: touched out-of-scope files:$bad"
  exit 0
fi

git add -A && git commit -q -m "loop: $desc" || { echo "ERROR: commit failed"; exit 2; }
sha="$(git rev-parse --short HEAD)"
revert() { git reset -q --hard HEAD~1; }

if ! timeout "$TIMEOUT_SEC" bash -c "$GUARD_CMD" > "$LOG" 2>&1; then
  revert; log "$sha" "" guard_fail "$desc"
  echo "REVERT guard_fail ($sha). Last output:"; tail -n 15 "$LOG"; exit 0
fi

m="$(run_metric)"; rc=$?
if [ $rc -ne 0 ]; then
  revert; log "$sha" "" eval_fail "$desc"
  echo "REVERT eval_fail rc=$rc ($sha). Last output:"; tail -n 15 "$LOG"; exit 0
fi

best="$(best_metric)"
if improved "$m" "$best"; then
  log "$sha" "$m" keep "$desc"
  echo "KEEP $METRIC_NAME=$m (previous best $best) commit=$sha"
  if [ -n "$TARGET" ] && awk -v n="$m" -v t="$TARGET" -v d="$DIRECTION" \
       'BEGIN { if (d=="lower") exit !(n <= t); else exit !(n >= t) }'; then
    echo "TARGET_REACHED $METRIC_NAME=$m (target $TARGET)"
  fi
else
  revert; log "$sha" "$m" discard "$desc"
  echo "DISCARD $METRIC_NAME=$m (best stays $best)"
fi
