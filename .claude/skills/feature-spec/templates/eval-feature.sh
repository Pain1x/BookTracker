#!/usr/bin/env bash
# Feature-mode eval: counts passing acceptance tests.
# Prints PASSED/TOTAL info lines and the single METRIC=<passed> line the loop parses.
# Exit 1 ONLY for infrastructure failure (build broken, no results). Failing tests are normal.
set -u
TEST_PROJECT="${TEST_PROJECT:-tests/MyApp.Tests}"        # <-- set
FILTER="${ACCEPT_FILTER:-Category=Accept_myfeature}"      # <-- set (matches the trait/category on the acceptance tests)
OUT=.karpathy/trx
rm -rf "$OUT"; mkdir -p "$OUT"

if ! dotnet build "$TEST_PROJECT" -nologo -v q > .karpathy/build.log 2>&1; then
  echo "BUILD FAILED"; tail -n 30 .karpathy/build.log; exit 1
fi
dotnet test "$TEST_PROJECT" --no-build --nologo --filter "$FILTER" \
  --logger "trx;LogFileName=accept.trx" --results-directory "$OUT" > .karpathy/test.log 2>&1

trx="$(ls "$OUT"/*.trx 2>/dev/null | head -1)"
[ -n "$trx" ] || { echo "NO TRX PRODUCED"; tail -n 30 .karpathy/test.log; exit 1; }

results="$(grep -o '<UnitTestResult [^>]*>' "$trx")"
passed="$(echo "$results" | grep -c 'outcome="Passed"')"
total="$(echo "$results" | grep -c '<UnitTestResult')"
[ "$total" -gt 0 ] || { echo "NO ACCEPTANCE TESTS MATCHED FILTER: $FILTER"; exit 1; }

echo "$results" | grep 'outcome="Failed"' | sed -E 's/.*testName="([^"]*)".*/\1/' > .karpathy/failing.txt
echo "TOTAL=$total"
echo "FAILING=$(wc -l < .karpathy/failing.txt) (names in .karpathy/failing.txt)"
echo "METRIC=$passed"
