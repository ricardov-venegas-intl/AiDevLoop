# Code Review Report: TASK-018

## Summary

`ClaudeLLMClient` is a correct, minimal adapter implementation with solid test coverage for the happy path and primary error paths. Three issues warrant attention before merging: inconsistent null-guard style, incomplete prompt escaping for edge-case inputs, and a missing null-argument test.

## Issues Found

### Blocking
- None

### Non-Blocking

- **Inconsistent null-guard pattern** (`ClaudeLLMClient.cs`, constructor vs `InvokeAsync`): The constructor uses the old `?? throw new ArgumentNullException(nameof(processRunner))` pattern while `InvokeAsync` already uses the modern `ArgumentNullException.ThrowIfNull`. Per the style guide preference for modern C#, the constructor should be updated to `ArgumentNullException.ThrowIfNull(processRunner)` for consistency.

- **Incomplete prompt escaping — newlines not handled** (`ClaudeLLMClient.cs`, `InvokeAsync`): Only `"` → `\"` is escaped. A prompt containing `\r`, `\n`, or `\r\n` will embed literal newlines inside the quoted argument string. Depending on how the concrete `IProcessRunner` constructs `ProcessStartInfo` (via `Arguments` string vs `ArgumentList`), this silently truncates the prompt or causes an unexpected argument parse. At minimum, newlines should be escaped (`\n` → `\\n`, etc.) or the prompt should be validated before constructing the argument string. A test covering multi-line prompts is also missing.

- **No test for null prompt** (`ClaudeLLMClientTests.cs`): `InvokeAsync` has `ArgumentNullException.ThrowIfNull(prompt)` but there is no test asserting it throws `ArgumentNullException` when `null` is passed. With `TreatWarningsAsErrors` and `Nullable` enabled the compiler limits accidental nulls, but an explicit test is still expected per the coverage pattern established in other adapter tests.

### Nitpicks

- **Redundant `using` directives** (`ClaudeLLMClient.cs`): `using System;`, `using System.Threading;`, and `using System.Threading.Tasks;` are all covered by implicit usings (enabled globally via `Directory.Build.props`). Only `using AiDevLoop.Core.Domain;` is needed. These don't cause a build failure today (no `EnforceCodeStyleInBuild=true`), but they're noise and inconsistent with the rest of the Shell adapters.

- **Expression-bodied constructor** (`ClaudeLLMClient.cs`): The `.editorconfig` enables `csharp_style_expression_bodied_constructors = true:silent`. The constructor body is a single assignment and could be written as `=> _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));` (or after fixing the null-guard, `ArgumentNullException.ThrowIfNull(processRunner); _processRunner = processRunner;` — expression-body not applicable there). This is purely cosmetic given the `:silent` severity.

## Compliance

- [x] Architecture adherence — Shell adapter only; no Core changes; correct project reference
- [x] Code style compliance — minor deviations (redundant usings, inconsistent null-guard) but no hard violations
- [ ] Test coverage adequate — 7 tests present but null-prompt and multi-line-prompt cases are missing
- [ ] Risk areas addressed — prompt escaping leaves newlines unhandled; noted in non-blocking issues
