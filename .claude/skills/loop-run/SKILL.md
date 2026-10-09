---
name: loop-run
description: Run the Karpathy autoresearch loop for N iterations on a project already set up with /loop-init. Each iteration a fresh loop-experimenter subagent makes one change, a deterministic script measures it and keeps or reverts via git. Use when the user says "run the loop", "start the karpathy loop", "iterate on this overnight", "continue the loop", or invokes /loop-run [N]. Also use to resume after a stop. Requires .karpathy/config.env to exist.
---

# loop-run: orchestrate the loop

You are the **orchestrator**. You do not edit code and you do not judge results. The loop is:

```
repeat:
  experimenter (subagent) -> one edit -> loop-step.sh -> KEEP | DISCARD | REVERT
  every 3 KEEPs -> loop-guard (subagent) audits the kept diffs
  check stop conditions
```

## Preflight

1. `.karpathy/config.env` and `.karpathy/program.md` exist, and `results.tsv` has a `baseline` row. If not: tell the user to run `/loop-init`.
2. `git status --porcelain` is empty and the branch is `loop/*`. If not, stop and say why. Never run the loop on `main`.
3. Read `config.env` and `program.md` so you can state the goal in one line.

## Parameters

`N` = iterations (from the user's argument, default **10**). Stop early on any of:
- **Plateau**: 6 consecutive non-KEEP outcomes (8 in feature mode, where progress is lumpier).
- **Target reached**: `loop-step.sh` prints `TARGET_REACHED` (the config's `TARGET`). Stop, then run the final audit below.
- **Tamper signal**: any `rejected_scope` row, or loop-guard returns `FLAG`. Pause and report; do not auto-continue.
- **Repeated eval/guard failure**: 3 in a row means something is broken (flaky harness, broken env). Stop and report.

## Each iteration

1. Spawn the `loop-experimenter` subagent. Prompt: `Iteration <i> of <N>. Do one experiment per your instructions.` Give it nothing else; it reads `program.md`, `results.tsv` and `git log` itself, which is the loop's memory.
2. Read its one-line report. The authoritative outcome is the **last line of `results.tsv`**, not the subagent's claim. If they disagree, trust the file.
3. Print one progress line: `i/N  <status>  <metric>  <description>`.
4. After every 3rd KEEP (and once at the end if there was any KEEP), spawn `loop-guard` with the commit range since the last audit (`git log --oneline <last-audited>..HEAD`). If it replies `FLAG`, run `git revert --no-edit <sha>` for each flagged commit, append a `guard_revert` row to `results.tsv`, and stop per the tamper rule.

## Feature mode (`Mode: feature` in program.md)

Same loop, with these differences:
- Audit with `loop-guard` after **every** KEEP, not every third, until the target is reached. Gaming appears early and compounds.
- On `TARGET_REACHED`: run `loop-guard` over the whole branch (`git log --oneline <baseline-commit>..HEAD`), then run the full test suite once (`dotnet test`, no filter) and report the result.
- In the final report, say that "all N pass" means the code satisfies the tests, not necessarily the feature, and suggest the human add 2-3 unseen tests as a generalization check before merging.
- If the loop plateaus with tests still failing, list the failing test names from `.karpathy/failing.txt` and offer `loop-strategist`; typical causes are an ambiguous test, a missing API capability, or a test that conflicts with another.

## Final report (keep it short)

- Baseline -> best metric, absolute and %.
- Counts: keep / discard / guard_fail / eval_fail / rejected_scope / no_change.
- The kept changes, one line each (from `git log --oneline`).
- Honest read: was the gain real, or within noise (`MIN_DELTA`)? Which ideas dried up? Offer to run the `loop-strategist` agent to suggest `program.md` edits.
- Remind the user that kept commits are on branch `loop/*`, nothing touched `main`, and the changes still deserve a normal code review before merging.

## Hard rules

- Never edit files yourself, never edit `.karpathy/`, never rerun `loop-step.sh` to "retry" a result.
- Never change `MIN_DELTA`, `SCOPE`, `GUARD_CMD` or the eval mid-run. Only the user changes the rules, between runs.
