# LLM-Assisted Development Methodology

---

## The Core Idea

Build software with LLM agents using structured task specifications as the contract between human intent and agent execution. Every task starts with a self-contained spec in the implementation plan, ends with verified code, and leaves a paper trail of decisions.

---

## Two Phases

### Phase 1 — Setup (do once per project)

Create the documents that all agents will reference throughout development. Each document is produced with an agent, then reviewed by you.

| Document | Purpose |
|---|---|
| `requirements.md` | What we're building, with acceptance criteria |
| `architecture.md` | How it's structured, with ADRs for key decisions |
| `data-models.md` | Database schemas and API contracts |
| `ui-design.md` | User flows and component specifications |
| `test-strategy.md` | Test types, coverage targets, CI approach |
| `code-style.md` | Naming, patterns, examples of good/bad code |
| `implementation-plan.md` | Tasks organized into milestones — each task is a complete spec |

These documents are the **source of truth** for every agent in Phase 2.

> **Note on `implementation-plan.md`:** This is the most important planning artifact. Writing good task entries *is* the creative/thinking work. Ambiguity resolved here means clean execution later. See the full format spec below.

---

### Phase 2 — Development Loop (repeat per task)

```
Select Task → Load Task → Implement → Validate → Review → Integrate → Document → Commit
```

**1. Select Task** — Pick the next `pending` task from `implementation-plan.md`. Confirm its dependencies are done.

**2. Load Task** — Copy the task block into `context/current-task.md`. This is the only setup before implementation — no prompt generation needed.

**3. Implement** — The implementation agent receives `current-task.md` plus all `@referenced` docs. It writes code, tests, and fills out the Context Handoff in `context/implementation-notes.md` (decisions made, risk areas, known limitations).

**4. Automated Validation** — Before any human review: lint, format, type-check, unit tests, build. If anything fails, fix it here. Nothing broken reaches the review step.

**5. Review Loop** — Up to 3 iterations:
- Agent A reviews code against the task's validation criteria and flags issues as **Blocking**, **Non-Blocking**, or **Nitpick**
- Agent B categorizes issues and recommends whether to fix or continue
- Agent C fixes blocking issues, then loop returns to Step 4

Exit the loop when no blocking issues remain. Log non-blocking items to the backlog.

**6. Integration Check** — Run the full test suite. Manual smoke test if it's user-facing. Benchmark if performance requirements exist.

**7. Update Documentation** — Mark task `done` in the plan. Update architecture docs if design changed. Update CHANGELOG. Archive `current-task.md`, `implementation-notes.md`, and `review.md` to `context/completed/`.

**8. Commit** — Conventional commit message referencing the task ID. Push to feature branch.

---

## Milestone Reviews

After each milestone (every 5–10 tasks), step back and check:

- Did we build what requirements specified?
- Does code follow the architecture?
- What technical debt needs a refactoring task?
- Were estimates accurate? Adjust the remaining plan.

---

## `implementation-plan.md` — Format Spec

The plan is organized into milestones. Each task entry must be self-contained — the implementation agent should be able to execute it using only the task block and the referenced docs, with no additional clarification.

**This means ambiguity must be resolved when writing the task, not at implementation time.**

### File structure

```markdown
# Implementation Plan

## Milestone 1 — [Name]

- [ ] TASK-001 · Simple · [Short title]
- [ ] TASK-002 · Medium · [Short title]
- [x] TASK-003 · Trivial · [Short title]

## Milestone 2 — [Name]
...

---

## Task Definitions

[Full task blocks in ID order]
```

### Task block format

````markdown
## TASK-001: [Short imperative title]

**Milestone:** 1 — [Milestone name]
**Status:** pending | in-progress | done | blocked
**Complexity:** Trivial | Simple | Medium | Complex
**Depends on:** TASK-XXX, TASK-XXX | —

### What to build
[1–4 sentences. Describe the outcome, not the steps. Be specific about
inputs, outputs, and behavior. If it's an API endpoint, name it. If it's
a component, describe what it renders and what events it emits.]

### Files in scope
- `path/to/file.ts` (create | modify | read-only reference)
- `path/to/other.ts` (create | modify | read-only reference)

### Constraints
- [Hard limits: libraries to use or avoid, patterns to follow, performance
  requirements, security rules, anything the agent must not deviate from]
- [Reference docs/code-style.md for anything covered there rather than
  repeating it here]

### Validation criteria (Definition of Done)
- [ ] [Specific, testable outcome — not "works correctly"]
- [ ] [Each criterion maps to something lint, tests, or manual review can verify]
- [ ] Tests cover [specific cases]
- [ ] No lint/type errors

### Context references
- `docs/[relevant-doc].md#[section]`
- `docs/code-style.md`
````

### Complexity guide

| Level | Duration | Scope | Notes |
|---|---|---|---|
| **Trivial** | 15–30 min | 1 file, obvious change | Automated checks only |
| **Simple** | 1–2 hours | 1–3 files, existing patterns | Implementation + automated validation |
| **Medium** | 3–6 hours | 3–10 files, some design | Implementation + 1 review iteration |
| **Complex** | 1–2 days | 10+ files, architectural impact | **Break into smaller tasks first** |

If a task feels Complex, don't write one big task entry — decompose it into Simple/Medium tasks and add them to the plan.

### Example task

````markdown
## TASK-004: Add POST /auth/login endpoint

**Milestone:** 1 — Core Auth
**Status:** pending
**Complexity:** Simple
**Depends on:** TASK-001

### What to build
An endpoint that accepts `{ email, password }`, validates credentials
against the users table, and returns a signed JWT on success.

### Files in scope
- `src/routes/auth.ts` (create)
- `src/middleware/jwt.ts` (create)
- `src/models/user.ts` (read-only reference)

### Constraints
- Use the JWT library already in package.json — no new dependencies
- Follow the error envelope format in `docs/data-models.md#api-errors`
- Token expiry: 24h
- Passwords are bcrypt-hashed — use the existing `src/utils/crypto.ts` helper

### Validation criteria (Definition of Done)
- [ ] Returns 200 + `{ token }` on valid credentials
- [ ] Returns 401 with standard error body on invalid credentials
- [ ] Returns 400 if email or password field is missing
- [ ] Unit tests cover all three branches above
- [ ] No lint/type errors

### Context references
- `docs/architecture.md#auth`
- `docs/data-models.md#users`
- `docs/data-models.md#api-errors`
- `docs/code-style.md`
````

---

## Reusable Prompts

All prompts are fixed templates. They don't change between tasks — the task entry provides all variable content.

```
project/
├── docs/                         ← Source of truth for all agents
├── context/
│   ├── current-task.md           ← Active task (copied from implementation-plan.md)
│   ├── implementation-notes.md   ← Agent's decision log (Context Handoff)
│   ├── review.md                 ← Current review findings
│   └── completed/                ← Archived task history
└── prompts/
    ├── implement-task.prompt.md  ← Fixed implementation prompt
    ├── code-review.prompt.md     ← Fixed review prompt
    └── fix-issues.prompt.md      ← Fixed fix prompt
```

### `implement-task.prompt.md`

```
You are implementing a software task. Your inputs are:
- context/current-task.md — the complete task specification
- All docs referenced in the task's "Context references" section

Instructions:
1. Read the task specification fully before writing any code.
2. Implement exactly what is specified. Do not add features not listed.
3. Follow all constraints without exception.
4. Write tests that cover all validation criteria.
5. When done, fill out context/implementation-notes.md using this template:

---
## Context Handoff

**Task:** [TASK-ID]

**Decisions made:**
[Any design choices not dictated by the spec, and why you made them]

**Risk areas:**
[Parts of the implementation that are fragile, make assumptions, or
could break if something upstream changes]

**Known limitations:**
[Anything the implementation doesn't handle that a future task should address]
---
```

### `code-review.prompt.md`

```
You are reviewing an implementation. Your inputs are:
- context/current-task.md — the task specification and validation criteria
- context/implementation-notes.md — the implementer's decision log
- The code diff / changed files

For each issue found, classify it as:
- BLOCKING: violates a validation criterion, constraint, or causes incorrect behavior
- NON-BLOCKING: correct but suboptimal; log to backlog
- NITPICK: style preference; note only, do not block

Output your findings to context/review.md. If there are no blocking issues,
state "APPROVED" clearly at the top.
```

### `fix-issues.prompt.md`

```
You are fixing blocking issues from a code review. Your inputs are:
- context/current-task.md — the original task specification
- context/review.md — the review findings
- context/implementation-notes.md — the implementer's decision log

Fix all BLOCKING issues. Do not change anything unrelated to a blocking issue.
Update context/implementation-notes.md if any decisions changed.
```

---

## Why It Works

**Self-contained task entries** give agents complete, unambiguous specifications. The planning session is where thinking happens — execution is mechanical.

**Validation criteria** make requirements testable. The review agent has an objective checklist, not a gut feeling.

**Context handoff** preserves implementation decisions so the review agent understands *why* code is written a certain way, not just *what* it does.

**Complexity assessment upfront** prevents over-engineering simple tasks and under-specifying complex ones.

**Fixed, reusable prompts** eliminate prompt drift. The prompts improve over time through deliberate editing, not ad-hoc variation per task.

**Documentation discipline** keeps agents grounded in current reality rather than stale assumptions.

---

## Anti-Patterns to Avoid

- Writing vague task entries and hoping the agent figures it out
- Marking a task `in-progress` before its dependencies are `done`
- Skipping automated validation and going straight to review
- Letting the review loop run more than 3 iterations without human intervention
- Committing with failing tests or lint errors
- Not filling out the Context Handoff — this kills review quality
- Writing a Complex task instead of decomposing it

---

## Getting Started

1. Run Setup Phase to create your seven foundation documents
2. Write the first milestone's task entries in `implementation-plan.md` — be specific
3. Try the Development Loop on one small task
4. Observe what the review agent catches
5. Refine your task entry quality and prompts based on patterns you see
6. Scale to full project once the loop feels natural
