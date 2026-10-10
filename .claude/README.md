# Karpathy loop kit for software engineering (Claude Code)

Based on Karpathy's **autoresearch** pattern: an agent edits code, a fixed harness measures ONE number,
the change is kept if the number improves and reverted via git if not, repeat. The human steers by
editing `program.md`, not by writing code.

> Note: the MindStudio article describes a generic generator/critic loop. Karpathy's actual repo is stricter
> and more useful: a *measured* metric decides, never an LLM's opinion. This kit implements the strict version.

## What's inside
| File | Role |
|---|---|
| `skills/loop-init` | `/loop-init` interview + scaffold + noise measurement + baseline |
| `skills/feature-spec` | `/feature-spec` **new-feature mode**: agree API, write failing acceptance tests (the spec), stubs, eval that counts passing tests |
| `skills/loop-run` | `/loop-run [N]` orchestrator |
| `skills/loop-run/scripts/loop-step.sh` | Deterministic judge: scope check -> commit -> guard -> eval -> keep/revert -> `results.tsv` |
| `agents/loop-experimenter` | Makes ONE change per iteration (fresh context each time) |
| `agents/loop-guard` | Independent read-only auditor for metric gaming |
| `agents/loop-strategist` | Reads results, proposes `program.md` edits for the next run |
| `skills/loop-run/references/dotnet-metrics.md` | .NET metric recipes: warnings, BenchmarkDotNet, coverage, test time |

## Two modes
| | Optimize (`/loop-init`) | Feature (`/feature-spec`) |
|---|---|---|
| Goal | Improve a number on working code | Add new behavior |
| Metric | warnings, allocations, time... | acceptance tests passing (higher) |
| Who writes the spec | n/a | **You**, approved at a gate; tests are read-only to the agent |
| Done when | plateau or target | `TARGET_REACHED` (all N pass) |
| Main risk | metric gaming | test-shaped code / hollow implementation (loop-guard audits every keep) |

Feature workflow: `/feature-spec` -> review and approve tests -> `/loop-run 15` -> review diff like a PR -> add 2-3 tests the agent never saw.
Your tests cap the quality of the feature. Thin tests give a thin feature.

## Install (per project)
Copy the `.claude/` folder into the root of your repo. Needs: git, bash, `timeout` (coreutils), `awk`, and `jq` for the benchmark recipe.

## First trial (about 1 hour, to decide if it's worth your time)
1. Pick a **small, boring, measurable** target. Best first target: compiler warnings or allocations in one project.
2. `/loop-init` and answer the interview. Watch what it says about the noise floor.
3. `/loop-run 10`. Watch the first few iterations; check `.karpathy/results.tsv`.
4. Review `git log -p` on the `loop/*` branch like a normal PR. Then ask the `loop-strategist` agent for its verdict.

**It was worth it if**: the metric moved beyond the noise floor, the kept diffs survive your review, and the setup cost
(writing the eval + guard) is less than the time saved. **It wasn't if**: you spent most of the hour on the harness, wins
were noise, or kept changes were things you'd have rejected in review. That is a valid result: this loop only pays off on
targets with a cheap, trustworthy number.

## Safety properties
- Runs only on a `loop/*` branch; `main` is never touched.
- The agent cannot edit the eval, tests, or `.karpathy/` (auto-reverted, logged as `rejected_scope`).
- Keep/revert is a script, not the model's judgment.
- Kept commits are audited by a separate read-only agent for gaming.
- Cost: every iteration is an agent run plus a guard + eval run. Start with 10.
