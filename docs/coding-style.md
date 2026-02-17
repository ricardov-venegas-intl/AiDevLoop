C# Coding Style
===============

The general rule we follow is "use Visual Studio defaults". The repository's `.editorconfig` is authoritative for automated formatting and analyzer-driven style rules — this document describes the human-readable guidance that matches those settings.

1. We use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line. A single-line statement block may be written without braces only when the entire body is on a single line and the containing `if`/`else`/`else if` compound statement follows the single-line rule for every block (see rule 18). One exception: a `using` statement may be nested on the following line at the same indentation level.
2. We use four spaces of indentation (no tabs).
3. We use `_camelCase` for internal and private fields and use `readonly` where possible. Prefix internal and private instance fields with `_`, static fields with `s_`, and thread-static fields with `t_`. When used on static fields, `readonly` should come after `static` (e.g. `static readonly` not `readonly static`). Public fields should be used sparingly and should use PascalCasing with no prefix.
4. We avoid `this.` unless absolutely necessary.
5. We always specify the visibility, even if it's the default (e.g. `private string _foo` not `string _foo`). Visibility should be the first modifier (e.g. `public abstract` not `abstract public`).
6. Namespace imports should be specified at the top of the file, outside `namespace` declarations, and should be sorted alphabetically; place `System.*` namespaces before all others.
7. Avoid more than one empty line at any time. Do not leave multiple consecutive blank lines between members.
8. Avoid spurious whitespace (for example, prefer `if (someVar == 0)` not `if ( someVar == 0 )`).
9. If a file differs in style from these guidelines (for example private members are named `m_member` rather than `_member`), preserve the existing style within that file.
10. Use `var` only when the type is explicit on the right-hand side (for example: `var stream = new FileStream(...)`) or when the type is obvious from context. Target-typed `new()` is allowed only when the type appears on the same declaration line (e.g. `FileStream stream = new(...);`).
11. Use C# language keywords instead of framework/BCL type names (for example, `int`, `string`, `float`).
12. Name constant locals and fields with `PascalCase` (interop exceptions aside).
13. Use `PascalCase` for all method names, including local functions.
14. Prefer `nameof(...)` over string literals where appropriate (for parameter names, property names, etc.).
15. Place fields at the top of type declarations.
16. Use Unicode escape sequences (\uXXXX) instead of literal non-ASCII characters in source files.
17. When using labels (for `goto`), indent the label one less than the current indentation.
18. Single-statement `if` rules:
    - Never use single-line form that omits braces across multiple compound blocks (for example: `if (x) throw ...;` is disallowed when mixed with braced blocks).
    - Braces are always acceptable and required when any related block uses braces or when the statement body spans multiple lines.
    - Braces may be omitted only if the body of every block in the compound `if`/`else if`/.../`else` is on a single line.
19. Make internal and private types `static` or `sealed` unless derivation is required.
20. Primary constructor parameters should use `camelCase` (no `_` prefix). Assign to `_`-prefixed fields only when appropriate for larger types where explicit field assignment improves clarity.
21. Each type should be declared in its own file. Do not define multiple top-level or sibling types in a single source file; the only common exception is small, private or nested helper types that are tightly coupled to the containing type and improve readability.

## C# Best Practices

### DO's ✅
- Always generate XML documentation
- Use TimeProvider instead of DateTime.Now and DateTime.UtcNow
- Always use LINQ method syntax; never use LINQ query syntax.
- Each type should have its own file. Do not have multiple types defined in a single file.
- Use `record` for DTOs, messages, and domain entities
- Use `readonly record struct` for value objects
- Leverage pattern matching with `switch` expressions
- Enable and respect nullable reference types
- Use async/await for all I/O operations
- Accept `CancellationToken` in all async methods
- Use `Span<T>` and `Memory<T>` for high-performance scenarios
- Accept abstractions (`IEnumerable<T]`, `IReadOnlyList<T>`)
- Return appropriate interfaces or concrete types
- Use `ConfigureAwait(false)` in library code
- Pool buffers with `ArrayPool<T>` for large allocations
- Prefer composition over inheritance
- Avoid abstract base classes in application code

### DON'Ts ❌
- Don't use mutable classes when records work
- Don't use classes for value objects (use `readonly record struct`)
- Don't create deep inheritance hierarchies
- Don't ignore nullable reference type warnings
- Don't block on async code (`.Result`, `.Wait()`)
- Don't use `byte[]` when `Span<byte>` suffices
- Don't forget `CancellationToken` parameters
- Don't return mutable collections from APIs
- Don't use `string` concatenation in loops
- Don't allocate large arrays repeatedly (use `ArrayPool`)

Additional modern C# preferences enforced via `.editorconfig` (summary)
- Prefer expression-bodied members for simple methods, constructors, operators, properties, indexers, accessors, lambdas and local functions where it improves readability.
- Prefer `switch` expressions where they make code clearer.
- Prefer `null`-propagation and throw-expressions where appropriate (e.g. `x ?? throw new ...`).
- Prefer conditional delegate invocation (e.g. `handler?.Invoke(...)`).
- Prefer `auto-properties`, inferred tuple and anonymous-type member names when applicable.
- Analyzer preferences (naming, spacing, modifier order, etc.) are defined in `.editorconfig`; follow that file for exact, machine-enforced rules.

File header / license
- The repository `.editorconfig` includes a `file_header_template`; please keep source file headers consistent with that template.

EditorConfig is authoritative
- The `.editorconfig` at the repository root is the source of truth for automated formatting and analyzer rules; configure your editor to respect it (Visual Studio, VS Code, Rider, etc.).
