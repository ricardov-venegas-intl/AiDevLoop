# Implementation Notes: TASK-019

## Task Name
Implement Copilot LLM Client

## Decisions Made
- Mirrored `ClaudeLLMClient` exactly, only changing command (`"copilot"`), arguments format (`-p "<prompt>"` without `--print`), and error message text: keeps the surface area minimal and the two clients obviously consistent.

## Known Limitations
- No `--print` flag: the Copilot CLI non-interactive mode uses `-p` alone; if a future version changes its flag this needs updating.

## Risk Areas
- Copilot CLI availability: the `copilot` binary must be on PATH at runtime. Mitigation: same pattern as ClaudeLLMClient — failure surfaces as `InvalidOperationException` with exit code and stderr.

## Dependencies
- IProcessRunner (TASK-016)
- ILLMClient interface (TASK-003)
- ClaudeLLMClient (TASK-018) — same pattern

## Testing Notes
- Used in-memory `FakeProcessRunner` for deterministic tests
- 7 new tests covering: trimmed stdout, non-zero exit, empty stdout, whitespace stdout, command name assertion, argument assertion, and quote escaping
