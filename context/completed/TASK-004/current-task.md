# Task TASK-004: Implement ConfigurationLoader

## Description
A ConfigurationLoader that reads configuration from `.aidevloop.json` or `.aidevloop.yaml` in the project root directory. If neither exists, return a `Configuration` record with sensible defaults. Support merging command-line overrides (`--llm`, `--verbose`, `--config`) into the loaded configuration. When `--config` is specified, load from that path instead.

## Definition of Done
- [x] Loads valid `.aidevloop.json` correctly
- [x] Loads valid `.aidevloop.yaml` correctly
- [x] Returns defaults when no config file exists
- [x] CLI `--llm` and `--verbose` overrides take precedence over file value
- [x] `--config=path` loads from specified path
- [x] Returns error for malformed JSON/YAML (graceful, not exception)
- [x] Unit tests added and passing
- [x] Code follows coding-style.md conventions (XML docs, nullable-aware)

## Steps
1. Add `ConfigurationLoader` to `src/AiDevLoop.Cli/ConfigurationLoader.cs`.
2. Add `YamlDotNet` package to `AiDevLoop.Cli` for YAML support.
3. Add unit tests under `tests/AiDevLoop.Shell.Tests/ConfigurationLoaderTests.cs`.
4. Run `dotnet build` and `dotnet test`.
5. Write implementation notes and a brief code-review report.
6. Request user approval before committing the changes.

## Acceptance Criteria
- [x] `ConfigurationLoader.Load` returns `Result<Configuration,string>.Ok` with the expected values for JSON/YAML and defaults
- [x] CLI overrides applied (llm, verbose)
- [x] Malformed files return `Err` with a clear message
- [x] Tests cover JSON, YAML, CLI overrides, explicit config path, and malformed input
