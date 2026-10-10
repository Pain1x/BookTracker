# Program: implement feature to pass acceptance tests

## Objective
Metric: `acceptance_test_failures` (lower is better), measured by `.karpathy/eval.sh`.
Baseline: TBD (run `loop-step.sh --baseline`). Target: 0 failures (all tests pass).

## Scope
You may edit only:
- ${SCOPE}

Everything else, including tests, build props, analyzers, and `.karpathy/`, is read-only.

## Hard rules
- Behavior must not change. The guard (`${GUARD_CMD}`) must pass.
- No metric gaming: no special-casing eval inputs, no suppressing tests, no weakening checks.
- One idea per experiment. Small diffs. Simpler code at equal metric is preferred.

## Ideas to try
- Implement the public API to satisfy test requirements
- Handle edge cases and error conditions
- Add proper validation and data handling

## Known dead ends
- None yet

## Context
${CONTEXT}
