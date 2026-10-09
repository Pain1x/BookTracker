---
name: feature-spec
description: Set up the Karpathy loop for ADDING A NEW FEATURE (spec-driven / test-first autonomous implementation). Interviews the user, agrees the public API, writes failing acceptance tests as the spec, adds compiling stubs, builds the eval that counts passing acceptance tests, and records baseline 0. Use when the user says "implement this feature with the loop", "build X with the karpathy loop", "spec-driven loop", "let the agent implement against tests", or wants new functionality (not optimization) built autonomously. Run this BEFORE /loop-run. For optimizing an existing metric use /loop-init instead.
---

# feature-spec: turn a feature into a measurable loop

The loop needs a number. For a feature, the number is **how many acceptance tests pass**. So the quality of the feature is capped by the quality of the tests, and writing them is the human's job. You help, but you stop at an approval gate.

Division of labor (do not blur it):
- **Human**: what the feature is, the architecture and public API, approving the tests.
- **You (this skill)**: draft the tests and stubs, build the harness.
- **Loop**: fill in the implementation until the tests pass.

## Step 1: Understand (ask at most 5 short questions; infer the rest from the repo)

Read the solution layout, test framework (xUnit / NUnit / MSTest), existing conventions, and where this feature naturally lives. Then confirm only what's unclear: behavior and inputs/outputs, error cases, edge cases that matter, where it belongs, explicit non-goals.

If the feature can't be expressed as testable behavior (pure UI layout, "make it feel nicer", exploratory design), say so plainly and recommend normal Claude Code instead of this loop.

## Step 2: Agree the API surface BEFORE writing tests

Propose the public types and signatures (interfaces, method signatures, DTOs/records, exceptions) in a short list. The human may change them; architecture is theirs. Wait for a go-ahead.

## Step 3: Write the acceptance tests

In the test project, new file(s) under `Features/<Feature>/`. Mark every test with a category that is unique to this feature: xUnit `[Trait("Category","Accept_<slug>")]`, NUnit `[Category("Accept_<slug>")]`, MSTest `[TestCategory("Accept_<slug>")]`.

Quality rules (these are what make the loop trustworthy):
- 10 to 40 tests. Name them `T01_...`, `T02_...` in order of difficulty, simplest first, so progress is incremental rather than all-or-nothing.
- Test behavior through the public API only. No reliance on internals, no mocking the thing being built.
- **Defeat hardcoding**: for each behavior use several different inputs (Theory/TestCase rows, varied values), including edge and error cases. A solution that special-cases two inputs must fail on the third.
- Assert concrete outputs, not just "does not throw".
- Deterministic: no clock, randomness, network or ordering dependence unless injected.
- Cover failure paths (invalid input, exceptions, boundaries), not just the happy path.

## Step 4: Stubs so everything compiles

In the implementation project add the agreed types with bodies that `throw new NotImplementedException();`. Goal: the test project builds, and every acceptance test fails at baseline (metric 0).

## Step 5: APPROVAL GATE (mandatory)

Show the user: the API list, the test list (names + one line each), and what is NOT covered. Then **stop and ask for approval or edits**. Do not continue until they approve. Say explicitly: "These tests are the spec. The loop will make exactly these pass, nothing more."

## Step 6: Harness

Working tree must be clean after committing the tests and stubs. Create branch `loop/feature-<slug>` (from a clean commit containing the stubs and tests).

Create `.karpathy/`:
- `eval.sh` from `.claude/skills/feature-spec/templates/eval-feature.sh`
- `config.env` from `templates/config.feature.env`, filled in: `TEST_PROJECT`, the category filter, `SCOPE` (implementation paths only; never tests), `GUARD_CMD` (all tests except this feature's category).
- `program.md` from `templates/program.feature.md`, filled in with the description, the API/architecture decisions and the suggested test order.
- `.gitignore` with `results.tsv`, `last_run.log`, `trx/`, `*.log`, `failing.txt`.

Run `bash .karpathy/eval.sh` once. Expect `METRIC=0` and `TOTAL=<N>`. If the build fails or the metric is not 0, fix the stubs/tests first. Put `TARGET=<N>` in `config.env`.

Commit `.karpathy/` ("loop: feature harness"), then run `.claude/skills/loop-run/scripts/loop-step.sh --baseline`.

## Step 7: Hand off

Report: feature name, number of tests, the baseline (0/N), scope, guard. Tell the user honestly:
- Success means all N pass, which proves the code meets *these tests*, not that the feature is complete. Review the final diff like a PR, and write 2-3 extra tests yourself that the agent never saw, to check generalization.
- Suggest `/loop-run 15` as the first run. Do not start it yourself.
