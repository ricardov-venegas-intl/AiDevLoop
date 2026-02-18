# Copilot Instructions — AiDevLoop

## Project Overview

AiDevLoop is a .NET 10.0 CLI tool that automates the Phase 2 development loop of the LLM-Assisted Development Methodology. It orchestrates LLM agents (Claude CLI or Copilot CLI) to implement tasks from an `implementation-plan.md`, run validations, manage review-fix cycles, and commit with user approval.

## Architecture

The project follows **Functional Core, Imperative Shell** with a pipeline-based architecture.

**Functional Core** (`AiDevLoop.Core`): Pure functions only. No I/O, no side effects, no project references. All business logic and decision-making lives here. Domain types use `record` for DTOs/entities and `readonly record struct` for value objects. Error handling uses `Result<TValue, TError>` with discriminated unions (abstract record + sealed record subtypes).

**Imperative Shell** (`AiDevLoop.Shell`): Orchestrates I/O via adapter interfaces (`IFileOperations`, `IProcessRunner`, `ILLMClient`, `IGitClient`, `IConsoleIO`). References only `AiDevLoop.Core`. Uses exceptions for I/O errors.

**CLI** (`AiDevLoop.Cli`): Entry point, argument parsing, configuration loading, DI wiring. References both Core and Shell.

### Project References

- `AiDevLoop.Core` → nothing (zero dependencies)
- `AiDevLoop.Shell` → `AiDevLoop.Core`
- `AiDevLoop.Cli` → `AiDevLoop.Core` + `AiDevLoop.Shell`

### Module Layout

```
src/
├── AiDevLoop.Cli/          # Entry point, CLI parsing, config loading
├── AiDevLoop.Core/         # Pure functions, domain types (NO I/O)
│   └── Domain/             # Records, enums, value objects, error unions
└── AiDevLoop.Shell/        # I/O orchestration, adapter implementations
    ├── Adapters/           # Interface definitions + implementations
    └── Resources/          # Embedded prompt templates

tests/
├── AiDevLoop.Core.Tests/   # Unit tests (no mocking needed)
├── AiDevLoop.Shell.Tests/  # Integration tests (in-memory adapters)
└── AiDevLoop.E2E.Tests/    # End-to-end tests with fixtures
```

## C# Coding Style

Follow Visual Studio defaults. The `.editorconfig` is authoritative for formatting and analyzer rules.

**Formatting**: Allman braces, 4-space indentation, no tabs. `_camelCase` for private fields, `s_` prefix for static fields. Always specify visibility. `System.*` imports first.

**Type conventions**: Use `var` only when the type is obvious. Use language keywords (`int`, `string`) over BCL names. `PascalCase` for methods, constants, and local functions. `nameof(...)` over string literals.

**Modern C# preferences**: Expression-bodied members where readable, `switch` expressions, null-propagation, conditional delegate invocation, auto-properties.

**Each type gets its own file.** No multiple top-level types in one file.

### Best Practices

**DO**: XML documentation on all public APIs. `TimeProvider` instead of `DateTime.Now`. LINQ method syntax only (no query syntax). `record` for DTOs/entities. `readonly record struct` for value objects. Pattern matching with `switch` expressions. Nullable reference types enabled and respected. `async/await` with `CancellationToken` for all async I/O. `ConfigureAwait(false)` in library code. Composition over inheritance.

**DON'T**: Mutable classes when records work. Deep inheritance hierarchies. Blocking on async (`.Result`, `.Wait()`). Ignoring nullable warnings. `string` concatenation in loops. Mutable collections from public APIs. Abstract base classes in application code.

## Error Handling

**Core**: Return `Result<T, TError>` for expected failures. Error types are discriminated unions:
```csharp
public abstract record SelectionError;
public sealed record TaskNotFound(TaskId Id) : SelectionError;
public sealed record DependenciesNotMet(TaskId Id, IReadOnlyList<TaskId> Unsatisfied) : SelectionError;
```

**Shell**: Throw exceptions for I/O failures. The orchestrator catches and converts to user-friendly messages containing: what went wrong, why, and how to fix it.

## Testing

Core tests are pure unit tests — no mocking required, just call functions with inputs and assert outputs. Shell tests use in-memory implementations of adapter interfaces. E2E tests use mock helpers and temporary directories with real git repos.

Test framework: xUnit with `Microsoft.NET.Test.Sdk`.

## Key Design Decisions

- Single task execution only — no parallelism
- LLM integration via CLI tools (`claude`, `copilot`) through `IProcessRunner` — no direct API calls
- Atomic file writes (write to temp, then move)
- Text-based plan updates — simple string replacement, no LLM for status changes
- Step 8 (commit) is the only mandatory human checkpoint
- Single-file self-contained executable deployment
- `Result<T, TError>` in core, exceptions in shell

## Configuration

Config file: `.aidevloop.json` or `.aidevloop.yaml` in project root. CLI flags override file settings. Defaults: `llm="claude"`, `paths.docs="docs"`, `paths.context="context"`, `paths.prompts="prompts"`, `validation.maxReviewIterations=3`.

## Attribution

Do not add attribution, watermarks, or "Generated with" comments to any code or files.
