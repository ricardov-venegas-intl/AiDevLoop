# Task 015: Implement FileOperations with atomic writes

## Description
Concrete implementation of `IFileOperations`. Read files as strings, write files atomically (write to temp file then move), create directories recursively, copy and move files, check existence, list directory contents, and archive context files to `context/completed/{TASK-ID}/`.

## Definition of Done
- [ ] `ReadFile` returns file content as string
- [ ] `WriteFile` uses atomic write (creates temp file, then moves)
- [ ] `WriteFile` creates parent directories if they don't exist
- [ ] `FileExists` and `DirectoryExists` return correct results
- [ ] `CreateDirectory` creates nested directory structure
- [ ] `ArchiveContextFiles` moves all three context files to `completed/{TASK-ID}/`
- [ ] Cross-platform paths work correctly
- [ ] No lint/type errors
- [ ] Build succeeds in local environment
- [ ] Ready for code review

## Steps
1. Create `src/AiDevLoop.Shell/Adapters/FileOperations.cs` implementing `IFileOperations`
2. Atomic write: write to temp file in same directory, then `File.Move` with overwrite
3. UTF-8 encoding, no BOM for all text operations
4. `ArchiveContextFiles` moves `current-task.md`, `implementation-notes.md`, `review.md` from contextDir to archiveDir
5. `ListFiles` returns relative paths within the specified directory
6. Create `tests/AiDevLoop.Shell.Tests/FileOperationsTests.cs` using a temp directory
7. Run `dotnet build -warnaserror` and `dotnet test` to validate

## Acceptance Criteria
- [ ] `ReadFile` returns file content; throws meaningful exception if missing
- [ ] `WriteFile` uses atomic write (temp + move); creates parent dirs automatically
- [ ] `CopyFile` and `MoveFile` work correctly
- [ ] `FileExists` and `DirectoryExists` return correct booleans
- [ ] `CreateDirectory` creates nested structure
- [ ] `ArchiveContextFiles` moves the three context files to the archive directory
- [ ] `ListFiles` returns relative paths, respects optional pattern
- [ ] Cross-platform: uses `Path.Combine`, no hardcoded separators
- [ ] All public APIs have XML documentation
- [ ] Zero build warnings with `-warnaserror`
- [ ] All unit tests pass

## Context References
- `docs/architecture.md#adr-006-atomic-file-writes`
- `docs/architecture.md#component-breakdown`
- `docs/requirements.md#nfr-2-reliability`
- `docs/coding-style.md`
- `src/AiDevLoop.Shell/Adapters/IFileOperations.cs`
