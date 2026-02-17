# Requirements — AiDevLoop

## Overview

**AiDevLoop** is a command-line tool that automates Phase 2 (Development Loop) of the LLM-Assisted Development Methodology. It orchestrates LLM agents to implement tasks from start to commit, with a single human approval point before committing code.

The tool reads task specifications from `implementation-plan.md`, delegates work to LLM agents (claude cli or copilot cli), runs automated validations, and manages the review-fix cycle until the task meets its validation criteria.

**Target users:** Developers using the LLM-Assisted Development Methodology defined in `LLM-Assisted-Development-Methodology.md`.

---

## Functional Requirements

### FR-1: Command-Line Interface

**FR-1.1: Binary name and aliases**
- Primary command: `aidevloop`
- Alias: `adl`
- Both must invoke the same executable

**FR-1.2: Help system**
- `aidevloop --help` displays usage, commands, and flags
- `aidevloop run --help` shows run-specific options
- `aidevloop resume --help` shows resume-specific options

**FR-1.3: Version flag**
- `aidevloop --version` displays tool version

---

### FR-2: Project Validation

**FR-2.1: Phase 1 document verification**

Before executing any task, verify the following documents exist in the `docs/` directory:

**Mandatory documents** (missing = error, abort):
- `requirements.md`
- `architecture.md`
- `implementation-plan.md`

**Optional documents** (missing = warning to stdout, continue):
- `data-models.md`
- `ui-design.md`
- `test-strategy.md`
- `code-style.md`

**FR-2.2: Directory structure**
- `docs/` must exist (error if missing)
- `context/` is created automatically if it doesn't exist
- `context/completed/` is created automatically if it doesn't exist
- `prompts/` path is configurable (see FR-3)
- If `prompts/` folder doesn't exist or required prompt files are missing, create them with default templates:
  - `implement-task.prompt.md`
  - `code-review.prompt.md`
  - `fix-issues.prompt.md`

**FR-2.3: Implementation plan validation**
- `implementation-plan.md` must be valid markdown
- Must contain at least one task definition
- Task definitions must include: ID, Milestone, Status, Complexity, What to build, Files in scope, Constraints, Validation criteria, Context references

---

### FR-3: Configuration

**FR-3.1: Configuration file**
- Look for `.aidevloop.json` or `.aidevloop.yaml` in project root
- If not found, use sensible defaults

**FR-3.2: Configuration schema**

```json
{
  "llm": "claude" | "copilot",
  "paths": {
    "docs": "docs",
    "context": "context",
    "prompts": "prompts"
  },
  "validation": {
    "maxReviewIterations": 3,
    "commands": {
      "lint": "npm run lint",
      "typecheck": "npm run typecheck",
      "test": "npm test",
      "build": "npm run build"
    }
  },
  "verbose": false
}
```

**FR-3.3: Command-line overrides**
- `--llm=claude` or `--llm=copilot` overrides config file
- `--verbose` overrides config file
- `--config=path/to/config.json` uses alternate config file

---

### FR-4: Task Execution (`aidevloop run`)

**FR-4.1: Task selection**
- If no task ID provided: auto-select the next `pending` task whose dependencies are all `done`
- If task ID provided (e.g., `aidevloop run TASK-005`): use that specific task
- If specified task is not `pending`, error and abort
- If specified task has unsatisfied dependencies, error and list them

**FR-4.2: Development Loop execution**

**Automation:** Steps 1-7 are fully automated and executed by LLM agents. Step 8 requires mandatory user approval before committing. User interaction only occurs on failures (see FR-4.3).

Execute the 8-step Phase 2 loop:

1. **Select Task** (LLM-automated) — Identify next task (see FR-4.1)
2. **Load Task** (LLM-automated) — Copy task block from `implementation-plan.md` to `context/current-task.md`
3. **Implement** (LLM-automated) — Invoke LLM agent with `prompts/implement-task.prompt.md` + `current-task.md` + referenced docs. Agent writes code and fills `context/implementation-notes.md`
4. **Automated Validation** (LLM-automated) — Run lint, typecheck, test, build commands from config. If any fail, ask user whether to continue review loop or abort (see FR-4.3)
5. **Review Loop** (LLM-automated) — Up to `maxReviewIterations` (default 3):
   - Invoke review agent with `prompts/code-review.prompt.md` + task + implementation notes + code diff
   - Review agent writes findings to `context/review.md` with issue classifications (BLOCKING, NON-BLOCKING, NITPICK)
   - If no BLOCKING issues: exit review loop
   - If BLOCKING issues exist: invoke fix agent with `prompts/fix-issues.prompt.md` + review findings
   - Fix agent updates code and `implementation-notes.md`
   - Re-run automated validation (step 4)
   - If `maxReviewIterations` exhausted with BLOCKING issues still present: ask user whether to continue or abort (see FR-4.3)
6. **Integration Check** (LLM-automated) — Run full test suite (config `validation.commands.test` and `validation.commands.build`)
7. **Update Documentation** (LLM-automated) — Mark task as `done` in `implementation-plan.md`. Archive `current-task.md`, `implementation-notes.md`, and `review.md` to `context/completed/{TASK-ID}/`
8. **Commit** (USER APPROVAL REQUIRED) — Display proposed commit message (conventional commit format with task ID). **Pause for user approval.** On approval: commit changes. On rejection: abort without committing.

**FR-4.3: Interactive failure handling**

When automated validation fails or review loop is exhausted:
- Display error summary to user
- Prompt: `Continue with another review iteration? (y/n/abort)`
  - `y` — Run one more review iteration (resets the loop)
  - `n` — Skip to step 8 (user approval) with warnings
  - `abort` — Exit without committing, save state for resume

**FR-4.4: Status tracking**
- Update task status in `implementation-plan.md`:
  - `pending` → `in-progress` when step 3 begins
  - `in-progress` → `done` when step 7 completes
- If user aborts: task remains `in-progress`

---

### FR-5: Resume Execution (`aidevloop resume`)

**FR-5.1: State recovery**
- Check for `context/current-task.md`
- If not found: error "No task in progress"
- If found: determine last completed step by inspecting:
  - Presence of `context/implementation-notes.md` → implementation complete (step 3 done)
  - Presence of `context/review.md` → review complete (step 5 done)
  - Task status in `implementation-plan.md` (`in-progress` vs `pending`)

**FR-5.2: Resume from checkpoint**
- Resume execution from the next incomplete step in the loop
- Display: `Resuming TASK-XXX from step N`
- Continue through remaining steps as normal

**FR-5.3: Smart recovery**
- If state is ambiguous (e.g., partial files exist), prompt user: `Resume from step N? (y/n)`
- Allow user to specify step: `aidevloop resume --from-step=4`

---

### FR-6: LLM Agent Integration

**FR-6.1: Supported LLM CLIs**
- `claude` (Anthropic Claude CLI)
- `copilot` (GitHub Copilot CLI)

**FR-6.2: Agent invocation**
- Each agent invocation includes:
  - Relevant prompt file from `prompts/`
  - `context/current-task.md`
  - All files referenced in task's "Context references" section
  - Code diff (for review and fix agents)
- Tool constructs prompt by concatenating context files, then invokes LLM CLI
- Capture stdout/stderr from LLM agent

**FR-6.3: Agent failure handling**
- If LLM CLI returns non-zero exit code: display error, ask user to retry or abort
- If LLM output is malformed (e.g., doesn't write expected file): display error, ask user to retry or abort

---

### FR-7: Output and Logging

**FR-7.1: Normal mode**
- Display current step name and status (e.g., `[2/8] Load Task → context/current-task.md`)
- Show summary after each step completes
- Show errors and warnings clearly
- Display user prompts for approval/decisions

**FR-7.2: Verbose mode (`--verbose`)**
- Log full LLM prompts sent to agent
- Log full LLM responses
- Log file operations (read, write, copy)
- Log validation command output (lint, test results)

**FR-7.3: Quiet mode (`--quiet`)**
- Suppress step-by-step output
- Show only errors and user prompts

---

### FR-8: Error Handling

**FR-8.1: Graceful degradation**
- If optional Phase 1 docs are missing: warn but continue
- If validation commands are not defined in config: skip that validation step with warning

**FR-8.2: Clear error messages**
- File not found errors include expected path
- Task dependency errors list unsatisfied dependencies
- Validation errors include failing command and output excerpt

**FR-8.3: Exit codes**
- `0` — Success (task completed and committed)
- `1` — User aborted
- `2` — Configuration error (missing mandatory docs, invalid config)
- `3` — Task execution error (validation failure, LLM agent error)

---

## Non-Functional Requirements

### NFR-1: Performance
- Task selection scan of `implementation-plan.md` completes in <1s for plans with <1000 tasks
- File operations use streaming for large files

### NFR-2: Reliability
- Atomic file writes (write to temp, then move)
- State is always recoverable via `aidevloop resume` if process is interrupted

### NFR-3: Usability
- Help text is concise and includes examples
- Error messages are actionable (explain what's wrong and how to fix)
- User prompts have clear options and default values

### NFR-4: Compatibility
- Support Windows, macOS, Linux
- Developed in C# / .NET 10.0
- Distributed as self-contained executable (single-file deployment)
- Work with both `claude` and `copilot` CLI tools

---

## Acceptance Criteria

### AC-1: Project Setup
- [ ] Tool validates presence of mandatory Phase 1 docs before executing any task
- [ ] Tool warns (but continues) if optional docs are missing
- [ ] Tool creates `context/` and `context/completed/` if they don't exist

### AC-2: Task Execution Flow
- [ ] `aidevloop run` auto-selects next pending task with satisfied dependencies
- [ ] `aidevloop run TASK-005` executes that specific task
- [ ] Tool updates task status to `in-progress` when implementation begins
- [ ] Tool invokes implement agent with correct context files
- [ ] Tool runs all validation commands defined in config
- [ ] Tool invokes review agent and parses `review.md` for BLOCKING issues
- [ ] Tool invokes fix agent when BLOCKING issues exist
- [ ] Review loop exits after max iterations, prompting user
- [ ] Tool archives context files to `context/completed/{TASK-ID}/` on completion
- [ ] Tool updates task status to `done` in `implementation-plan.md`
- [ ] Tool pauses before commit for user approval
- [ ] Tool commits with conventional commit message referencing task ID

### AC-3: Resume Functionality
- [ ] `aidevloop resume` detects current task from `context/current-task.md`
- [ ] Tool resumes from correct step based on existing context files
- [ ] Tool prompts user if state is ambiguous
- [ ] `--from-step=N` flag allows manual step specification

### AC-4: Configuration and Overrides
- [ ] Tool loads config from `.aidevloop.json` or `.aidevloop.yaml`
- [ ] `--llm=` flag overrides config file setting
- [ ] `--verbose` flag enables detailed logging
- [ ] `--config=` flag uses alternate config file

### AC-5: Error Scenarios
- [ ] Tool errors if mandatory Phase 1 doc is missing
- [ ] Tool errors if specified task has unsatisfied dependencies
- [ ] Tool prompts user when validation fails (continue/abort)
- [ ] Tool prompts user when review loop exhausts iterations
- [ ] Tool returns correct exit codes (0=success, 1=abort, 2=config error, 3=execution error)

### AC-6: Output Quality
- [ ] Normal mode shows current step and summary
- [ ] Verbose mode logs full LLM interactions
- [ ] Error messages include actionable context
- [ ] `--help` displays usage, commands, and examples

---

## Out of Scope

The following are explicitly **not** part of this tool:

- **Phase 1 automation** — Generating setup documents is a separate concern
- **GUI or web interface** — CLI only
- **Task authoring** — Users write tasks manually in `implementation-plan.md`
- **Custom LLM integrations** — Only `claude` and `copilot` CLIs are supported
- **Multi-task parallel execution** — One task at a time
- **Rollback/undo** — Use git for version control
- **Milestone reviews** — Manual process, not automated
- **Prompt editing within tool** — Prompts are files, edited externally

---

## Dependencies

### External Tools
- **LLM CLI** — User must have `claude` or `copilot` CLI installed and configured
- **Git** — For commit operations (step 8)
- **Validation tools** — Linter, type-checker, test runner (project-specific, configured by user)

### File Requirements
- `implementation-plan.md` with valid task definitions
- Prompt files: `implement-task.prompt.md`, `code-review.prompt.md`, `fix-issues.prompt.md`

---

## Future Enhancements (Not in MVP)

- Support for additional LLM CLIs (e.g., OpenAI CLI, local models)
- Interactive task picker UI (`aidevloop run --interactive`)
- Automatic prompt generation/refinement based on task complexity
- Integration with issue trackers (GitHub Issues, Jira)
- Parallel execution of independent tasks
- Telemetry and analytics (task completion times, review iteration counts)
- Web dashboard for viewing task history and context handoffs
