# .NET metric recipes (adapt paths and project names)

Each recipe is an `EVAL_CMD` body that ends by printing `METRIC=<number>`. Guard is usually
`dotnet test --nologo -v q`. Put multi-line evals in `.karpathy/eval.sh` and set `EVAL_CMD='bash .karpathy/eval.sh'`.

## 1. Compiler warnings (lower). Fast, deterministic, low gaming risk.
```bash
n=$(dotnet build --no-incremental -nologo 2>&1 | grep -E '^\s+[0-9]+ Warning\(s\)' | tail -1 | awk '{print $1}')
echo "METRIC=${n:-0}"
```
Gaming to watch: `#pragma warning disable`, `<NoWarn>` edits, `[SuppressMessage]`. loop-guard checks these.
Keep `Directory.Build.props` out of SCOPE.

## 2. Allocations or time in a hot path (lower). BenchmarkDotNet.
```bash
dotnet run -c Release --project bench/Bench.csproj -- --filter '*' --exporters json >/dev/null 2>&1
f=$(ls -t BenchmarkDotNet.Artifacts/results/*-report-full.json | head -1)
jq '[.Benchmarks[].Memory.BytesAllocatedPerOperation] | add' "$f" | sed 's/^/METRIC=/'
```
Prefer **allocated bytes** (deterministic) over mean time (noisy). If you use time:
`jq '[.Benchmarks[].Statistics.Mean] | add'` and set MIN_DELTA above your measured spread.
Check the exact JSON field names against your BenchmarkDotNet version before the first run.
Gaming to watch: caching keyed to benchmark inputs, changing benchmarked inputs. Keep `bench/` out of SCOPE.

## 3. Line coverage (higher). Weakest metric, easy to game with assertion-free tests.
```bash
rm -rf .karpathy/cov
dotnet test --nologo -v q --collect:"XPlat Code Coverage" --results-directory .karpathy/cov >/dev/null 2>&1
f=$(ls -t .karpathy/cov/*/coverage.cobertura.xml | head -1)
grep -o 'line-rate="[0-9.]*"' "$f" | head -1 | grep -o '[0-9.]*' | awk '{printf "METRIC=%.4f\n", $1*100}'
```
Needs the `coverlet.collector` package in the test project. Here SCOPE is the *test* folder, so
the guard cannot be "tests pass" alone: add a mutation-testing check or rely on loop-guard review for
assertion-free tests. Consider this recipe only after the others.

## 4. Test suite duration (lower). Only if tests are deterministic.
```bash
s=$(date +%s.%N); dotnet test --nologo -v q --no-build >/dev/null 2>&1 || exit 1; e=$(date +%s.%N)
echo "METRIC=$(echo "$e - $s" | bc -l)"
```
Noisy: measure the spread in loop-init and set MIN_DELTA accordingly.

## Choosing
Start with 1 or 2. They have a real, checkable number and the guard (tests) stays meaningful.
Avoid vague targets ("cleaner architecture"): no scalar, no loop.

## 5. Acceptance tests passing (higher). FEATURE MODE
Not a recipe to copy here: use `/feature-spec`. It generates `eval.sh` that builds the test project, runs only the
feature's category, parses the .trx and prints `METRIC=<passed>`. Deterministic, so `MIN_DELTA=0`.
