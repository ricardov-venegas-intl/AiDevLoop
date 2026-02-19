# Code Review Report: TASK-017

## Summary

TASK-017 (`OutputMode` enum + `ConsoleIO` implementation) is well-structured and meets the stated task requirements. No blocking issues were found; there are three non-blocking issues worth addressing and two nitpicks.

## Issues Found

### Blocking

None.

### Non-Blocking

**1. Public constructor duplicates field assignments instead of delegating**

The public constructor and the private constructor assign the same four fields independently. The public ctor should chain to the private one:

```csharp
// current — duplicated body
public ConsoleIO(OutputMode mode, TextWriter output, TextReader input)
{
    _mode = mode;
    _output = output;
    _input = input;
    _useColor = false;
}

// preferred — single source of truth
public ConsoleIO(OutputMode mode, TextWriter output, TextReader input)
    : this(mode, output, input, useColor: false) { }
```

**2. `WriteError` and `WriteWarning` write to stdout, not stderr**

`CreateForConsole` injects `Console.Out` as `_output`, so errors and warnings are written to stdout. In a CLI tool, error output should go to `Console.Error` so that callers can separate the two streams. The shell adapter should inject a second `TextWriter _error` (defaulting to `Console.Error`) and use it in `WriteError` / `WriteWarning`.

**3. `PromptChoice<T>` enters an infinite loop on empty `options` or EOF**

- If `options.Count == 0`, no integer in `[1, 0]` can ever be valid so the loop never exits.
- If `_input` reaches EOF, `ReadLine()` returns `null`, `int.TryParse` fails, and the loop spins forever.

A guard at entry covers the empty case:

```csharp
if (options.Count == 0)
    throw new ArgumentException("Options list must not be empty.", nameof(options));
```

For the EOF case, a null-check on `line` (throwing or writing an error before re-prompting) would be appropriate.

### Nitpicks

**1. Redundant `using` directives**

`Directory.Build.props` sets `<ImplicitUsings>enable</ImplicitUsings>`, which auto-generates global `using` declarations for `System`, `System.Collections.Generic`, `System.IO`, and others. The explicit directives in both `ConsoleIO.cs` and `ConsoleIOTests.cs` are therefore redundant. They won't fail the build (IDE0005 is suggestion-level, not a compiler warning), but removing them keeps the files consistent with the rest of the codebase.

**2. `WriteVerbose` behavior diverges silently between color and non-color modes**

In non-color mode the output includes a `[VERBOSE]` prefix; in color mode it does not — only the color differentiates the output. The behavioral difference is undocumented and could confuse future maintainers. Consider applying the prefix in both paths, or noting the divergent behavior in the XML doc.

## Compliance

- [x] Architecture adherence — `OutputMode` is a pure enum in `Core.Domain`; `ConsoleIO` is in `Shell.Adapters` and depends only on `System.*` and `Core.Domain`. Core stays pure.
- [x] Code style — Allman braces, `_camelCase` fields, `sealed` class, XML docs on all public members, one type per file.
- [x] Test coverage — 20 tests covering all modes for each write method, both `Confirm` outcomes, `PromptChoice` happy path and retry loop. Coverage is adequate. The empty-options and EOF edge cases are not tested (follow-on to non-blocking issue 3).
- [ ] Risk areas addressed — `WriteError`/`WriteWarning` targeting stdout (non-blocking issue 2) and the infinite-loop risk in `PromptChoice` (non-blocking issue 3) should be addressed before the implementation is considered fully hardened.
