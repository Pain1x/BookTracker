# Program: implement feature "<name>" so that all acceptance tests pass

Mode: feature

## Objective
Metric: `acceptance_tests_passing` (higher is better). Target: all <TOTAL> tests.
The acceptance tests in `<path>` ARE the specification. Make them pass by implementing real behavior.

## Feature description (human-written, authoritative)
<what the feature does, in plain words; inputs, outputs, errors, non-goals>

## Architecture decisions (made by the human, not up for debate)
- Public API: <signatures / interfaces already stubbed>
- Placement: <namespace/folder>
- Conventions to follow: <patterns used in this codebase: result types, DI style, logging, async rules>
- Must NOT: <e.g. add packages, change public API of existing types, touch other features>

## Hard rules
- Tests, test helpers, `.karpathy/`, build props and analyzers are read-only. Edits outside scope are auto-reverted.
- Implement the intended behavior generally. Passing a test by returning its expected value, branching on test inputs,
  detecting the test environment, or catching and swallowing exceptions is cheating and will be audited and reverted.
- Existing tests (regression guard) must keep passing.
- One increment per experiment: pick the simplest failing test group, make it pass, keep the code clean.

## Suggested order (simplest first)
1. <tests T01-T05: happy path basics>
2. <T06-T12: variations and edge cases>
3. <T13+: error handling, boundaries, concurrency if any>

## Notes / traps the agent can't infer
- <domain rules, gotchas, reference docs in repo>

## Known dead ends (fill between runs)
-
