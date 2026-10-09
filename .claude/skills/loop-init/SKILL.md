---
name: loop-init
description: Set up a Karpathy-style autonomous improvement loop (autoresearch pattern) for a software-engineering target. Use when the user says "set up a karpathy loop", "autoresearch this", "let the agent iterate overnight on X", "optimize X with an agent loop", or wants an agent to repeatedly try changes and keep only the ones that improve a measurable number (performance, warnings, coverage, bundle size, test time). Run this BEFORE /loop-run. Interviews the user, builds the fixed eval harness, measures metric noise, records a baseline.
---

# loop-init: scaffold a keep-or-revert loop

The Karpathy loop only works when four things are true. Your job is to make them true, or tell the user honestly that the target is a poor fit.

1. **ONE scalar metric**, computed by a command, not by an LLM's opinion.
2. **A bounded edit scope** (the files the agent may change).
3. **A hard guard** (e.g. all tests pass) so "better number" can't mean "broken code".
4. **A fixed harness the agent cannot edit** (eval script, benchmarks, tests that define the guard).

## Step 1: Interview (ask only what you can't infer from the repo)

- **Target**: what should get better? Push for something measurable. If the answer is "code quality" or "readability", say it doesn't fit and propose a proxy (warning count, cyclomatic complexity, LOC of a module, mutation score) or suggest not using this loop.
- **Direction**: lower or higher is better.
- **Scope**: which paths may be edited? Keep it small (one project / one folder). Tests, benchmarks, eval scripts, CI config, `.karpathy/` are ALWAYS out of scope.
- **Guard**: command that must pass for a change to count (usually the test suite). Must be fast enough to run every iteration.
- **Budget**: wall-clock limit per iteration (`TIMEOUT_SEC`).

For .NET recipes read `references/dotnet-metrics.md` (in `.claude/skills/loop-run/references/`) and adapt, don't copy blindly.

## Step 2: Create files

Work on a fresh branch: `git switch -c loop/<short-tag>`. Working tree must be clean.

Create `.karpathy/`:
- `config.env` (see `.claude/skills/loop-init/templates/config.env`): `METRIC_NAME`, `DIRECTION`, `EVAL_CMD`, `GUARD_CMD`, `SCOPE` (space-separated globs), `MIN_DELTA`, `TIMEOUT_SEC`.
- `eval.sh` if the eval is more than one line. It must print exactly one line `METRIC=<number>` as its LAST METRIC line. Everything else goes to stderr/stdout noise; only `METRIC=` is parsed.
- `program.md` from `.claude/skills/loop-init/templates/program.md`, filled in. This is the human-editable research direction. Be specific: what has been tried, what ideas to explore, hard rules.
- `.gitignore` containing `results.tsv` and `last_run.log`.

Commit all of `.karpathy/` ("loop: harness"). From now on any edit under `.karpathy/` is auto-rejected by the step script, so the agent cannot rewrite its own judge.

## Step 3: Measure noise (do not skip)

Run `EVAL_CMD` 3 times on untouched code. If the values differ, set `MIN_DELTA` to at least the observed spread (max minus min). A loop with `MIN_DELTA` below the noise floor just chases random fluctuations and reports fake wins. If the spread is large relative to the improvement the user hopes for, say so and propose a more stable metric (more benchmark iterations, deterministic counts like allocations or instructions instead of wall time).

## Step 4: Baseline

Run `.claude/skills/loop-run/scripts/loop-step.sh --baseline`. It checks the guard passes and records the starting number.

## Step 5: Hand off

Tell the user, briefly: the metric and its baseline, the noise floor / `MIN_DELTA`, the scope, the guard, and the cost caveat (each iteration is a full agent run plus an eval; estimate eval duration x iterations). Suggest `/loop-run 10` as a first trial, not 100. Do not start the loop yourself.
