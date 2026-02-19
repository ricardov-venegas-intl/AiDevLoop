# Implementation Notes: TASK-020

## Task Name
Implement GitClient

## Decisions Made
- Parses `git status --porcelain` line-by-line using the 2-char XY prefix: `line[0]` for index/staged state, `line[1]` for worktree/unstaged state, `line[3..]` for the file path. This is the simplest faithful interpretation of the porcelain format.
- Empty or whitespace-only stdout from `git status --porcelain` is treated as a clean repo and returns early with `new GitStatus(false, false, [])`, avoiding unnecessary iteration.
- Lines shorter than 3 characters are skipped to guard against trailing newlines that `Split('\n')` can produce.
- Quote-escaping in `CommitAsync` replaces `"` with `\"` in the message before embedding it in the argument string, matching the same approach used by `ClaudeLLMClient`.

## Known Limitations
- Does not support renamed/copied files specially (`R` and `C` status codes, which have a `->` separator in their path); those paths are collected verbatim as the `XY ` suffix, which may include the original and destination paths.
- Works only in the current working directory (no `workingDirectory` override) since `IProcessRunner.RunAsync(command, arguments, CancellationToken)` is used.

## Risk Areas
- Shell quoting on Windows: the message is embedded in `commit -m "..."` via `ProcessRunner`, which passes arguments as a raw string to the process. If the message contains shell meta-characters other than `"`, they could cause issues. Mitigation: this is consistent with how other clients in the shell layer handle arguments.

## Dependencies
- IProcessRunner (TASK-016)
- IGitClient interface (TASK-003)
- GitStatus domain type (TASK-002)

## Testing Notes
- Used in-memory FakeProcessRunner for deterministic tests
- git status --porcelain format: XY filename (2-char prefix + space + path)
