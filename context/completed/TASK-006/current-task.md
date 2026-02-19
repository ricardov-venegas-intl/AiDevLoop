# Task TASK-006: Implement CommandLineParser

## Description
A CommandLineParser that parses string[] args into a CommandLineArgs record. Support run and resume commands, an optional positional task ID argument for run, and flags: --llm=, --config=, --verbose, --quiet, --from-step=, --help, --version.

## Definition of Done
- Build succeeds in local environment (dotnet build)
- All tests pass (dotnet test)
- Code review completed with no blocking issues
- Ready for commit

## Steps
1. Create CommandLineParser.cs in src/AiDevLoop.Cli/
2. Implement manual string parsing (no external CLI libraries)
3. Return Result<CommandLineArgs, string> for errors
4. Create comprehensive test suite in tests/AiDevLoop.Shell.Tests/CommandLineParserTests.cs
5. Validate all constraints and edge cases
6. Run code review and address findings

## Acceptance Criteria
- [x] run parses to Command.Run with no task ID
- [x] run TASK-005 parses to Command.Run with TaskId("TASK-005")
- [x] resume parses to Command.Resume
- [x] resume --from-step=4 parses with step number 4
- [x] --llm=copilot sets LLM provider override
- [x] --verbose sets verbose flag
- [x] --quiet sets quiet flag
- [x] --verbose --quiet together returns error
- [x] --from-step with run command returns error
- [x] Unknown flags return error
- [x] No lint/type errors

## Quality Gate
- [x] Code builds without warnings (dotnet build -warnaserror)
- [x] All tests pass
- [x] XML docs on all public APIs
- [x] Code follows coding-style.md
- [x] One type per file
