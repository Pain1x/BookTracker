# Program: <one-line goal, e.g. "reduce allocations in OrderPipeline hot path">

## Objective
Metric: `<METRIC_NAME>` (<lower|higher> is better), measured by `.karpathy/eval.sh`.
Baseline: <fill after loop-step.sh --baseline>. Target (optional): <number>. Stop when reached.

## Scope
You may edit only: <SCOPE globs>. Everything else, including tests, benchmarks, build props,
analyzers and `.karpathy/`, is read-only. Edits outside scope are reverted automatically.

## Hard rules
- Behavior must not change. The guard (`<GUARD_CMD>`) must pass.
- No metric gaming: no special-casing eval inputs, no suppressing warnings/analyzers, no weakening checks.
- One idea per experiment. Small diffs. Simpler code at equal metric is not a win, but do prefer simple changes.

## Ideas to try (human-curated, edit freely between runs)
- <idea 1>
- <idea 2>

## Known dead ends (do not retry)
- <fill from previous runs / loop-strategist output>

## Context the agent can't infer
- <domain facts: invariants, hot paths, things that look redundant but aren't, perf-sensitive callers>
