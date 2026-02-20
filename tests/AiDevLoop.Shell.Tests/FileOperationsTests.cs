using System;
using System.IO;

using AiDevLoop.Shell.Adapters;

using Xunit;

namespace AiDevLoop.Shell.Tests;

/// <summary>
/// Tests for <see cref="FileOperations"/> using isolated real temp directories.
/// </summary>
public sealed class FileOperationsTests : IDisposable
{
    private readonly string _root;
    private readonly FileOperations _sut;

    /// <summary>
    /// Initializes a new instance of <see cref="FileOperationsTests"/>,
    /// creating a unique temp directory for each test.
    /// </summary>
    public FileOperationsTests()
    {
        _root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_root);
        _sut = new FileOperations();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    // ── ReadFile ────────────────────────────────────────────────────────────

    /// <summary>
    /// ReadFile returns the exact content that was previously written to the file.
    /// </summary>
    [Fact]
    public void ReadFile_ReturnsFileContent()
    {
        string path = Path.Combine(_root, "hello.txt");
        File.WriteAllText(path, "hello world");

        string result = _sut.ReadFile(path);

        Assert.Equal("hello world", result);
    }

    /// <summary>
    /// ReadFile throws <see cref="FileNotFoundException"/> when the file does not exist.
    /// </summary>
    [Fact]
    public void ReadFile_ThrowsFileNotFoundException_WhenMissing()
    {
        string path = Path.Combine(_root, "missing.txt");

        Assert.Throws<FileNotFoundException>(() => _sut.ReadFile(path));
    }

    // ── WriteFile ───────────────────────────────────────────────────────────

    /// <summary>
    /// WriteFile creates the file with the correct content.
    /// </summary>
    [Fact]
    public void WriteFile_WritesContent()
    {
        string path = Path.Combine(_root, "out.txt");

        _sut.WriteFile(path, "written content");

        Assert.Equal("written content", File.ReadAllText(path));
    }

    /// <summary>
    /// WriteFile creates intermediate directories when they do not exist.
    /// </summary>
    [Fact]
    public void WriteFile_CreatesParentDirectories()
    {
        string path = Path.Combine(_root, "a", "b", "c", "file.txt");

        _sut.WriteFile(path, "nested");

        Assert.True(File.Exists(path));
        Assert.Equal("nested", File.ReadAllText(path));
    }

    /// <summary>
    /// WriteFile overwrites an existing file with new content.
    /// </summary>
    [Fact]
    public void WriteFile_OverwritesExistingFile()
    {
        string path = Path.Combine(_root, "overwrite.txt");
        File.WriteAllText(path, "old content");

        _sut.WriteFile(path, "new content");

        Assert.Equal("new content", File.ReadAllText(path));
    }

    /// <summary>
    /// WriteFile does not emit a UTF-8 BOM.
    /// </summary>
    [Fact]
    public void WriteFile_WritesNoBom()
    {
        string path = Path.Combine(_root, "nobom.txt");

        _sut.WriteFile(path, "abc");

        byte[] bytes = File.ReadAllBytes(path);
        // UTF-8 BOM is 0xEF 0xBB 0xBF
        Assert.False(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
    }

    // ── CopyFile ────────────────────────────────────────────────────────────

    /// <summary>
    /// CopyFile copies the file to the destination path.
    /// </summary>
    [Fact]
    public void CopyFile_CopiesFileToDestination()
    {
        string src = Path.Combine(_root, "src.txt");
        string dest = Path.Combine(_root, "dest.txt");
        File.WriteAllText(src, "copy me");

        _sut.CopyFile(src, dest);

        Assert.True(File.Exists(dest));
        Assert.Equal("copy me", File.ReadAllText(dest));
        Assert.True(File.Exists(src), "source should still exist");
    }

    // ── MoveFile ────────────────────────────────────────────────────────────

    /// <summary>
    /// MoveFile moves the file to the destination and removes the source.
    /// </summary>
    [Fact]
    public void MoveFile_MovesFileToDestination()
    {
        string src = Path.Combine(_root, "source.txt");
        string dest = Path.Combine(_root, "moved.txt");
        File.WriteAllText(src, "move me");

        _sut.MoveFile(src, dest);

        Assert.True(File.Exists(dest));
        Assert.Equal("move me", File.ReadAllText(dest));
        Assert.False(File.Exists(src), "source should be gone");
    }

    // ── CreateDirectory ─────────────────────────────────────────────────────

    /// <summary>
    /// CreateDirectory creates nested directories successfully.
    /// </summary>
    [Fact]
    public void CreateDirectory_CreatesNestedDirectories()
    {
        string path = Path.Combine(_root, "x", "y", "z");

        _sut.CreateDirectory(path);

        Assert.True(Directory.Exists(path));
    }

    /// <summary>
    /// CreateDirectory does not throw when the directory already exists.
    /// </summary>
    [Fact]
    public void CreateDirectory_IsNoOpWhenAlreadyExists()
    {
        _sut.CreateDirectory(_root); // _root already exists

        Assert.True(Directory.Exists(_root));
    }

    // ── FileExists ──────────────────────────────────────────────────────────

    /// <summary>
    /// FileExists returns true for a file that exists.
    /// </summary>
    [Fact]
    public void FileExists_ReturnsTrueWhenFileExists()
    {
        string path = Path.Combine(_root, "exists.txt");
        File.WriteAllText(path, "");

        Assert.True(_sut.FileExists(path));
    }

    /// <summary>
    /// FileExists returns false for a file that does not exist.
    /// </summary>
    [Fact]
    public void FileExists_ReturnsFalseWhenFileMissing()
    {
        string path = Path.Combine(_root, "nothere.txt");

        Assert.False(_sut.FileExists(path));
    }

    // ── DirectoryExists ─────────────────────────────────────────────────────

    /// <summary>
    /// DirectoryExists returns true for a directory that exists.
    /// </summary>
    [Fact]
    public void DirectoryExists_ReturnsTrueWhenDirectoryExists()
    {
        Assert.True(_sut.DirectoryExists(_root));
    }

    /// <summary>
    /// DirectoryExists returns false for a directory that does not exist.
    /// </summary>
    [Fact]
    public void DirectoryExists_ReturnsFalseWhenDirectoryMissing()
    {
        string path = Path.Combine(_root, "ghost");

        Assert.False(_sut.DirectoryExists(path));
    }

    // ── ListFiles ───────────────────────────────────────────────────────────

    /// <summary>
    /// ListFiles returns relative file names for all files in the directory.
    /// </summary>
    [Fact]
    public void ListFiles_ReturnsRelativePaths()
    {
        File.WriteAllText(Path.Combine(_root, "a.txt"), "");
        File.WriteAllText(Path.Combine(_root, "b.txt"), "");

        IReadOnlyList<string> result = _sut.ListFiles(_root);

        Assert.Contains("a.txt", result);
        Assert.Contains("b.txt", result);
    }

    /// <summary>
    /// ListFiles with a pattern filters the results to matching files only.
    /// </summary>
    [Fact]
    public void ListFiles_WithPatternFiltersCorrectly()
    {
        File.WriteAllText(Path.Combine(_root, "keep.md"), "");
        File.WriteAllText(Path.Combine(_root, "drop.txt"), "");

        IReadOnlyList<string> result = _sut.ListFiles(_root, "*.md");

        Assert.Contains("keep.md", result);
        Assert.DoesNotContain("drop.txt", result);
    }

    // ── ArchiveContextFiles ─────────────────────────────────────────────────

    /// <summary>
    /// ArchiveContextFiles moves all three context files to the archive directory.
    /// </summary>
    [Fact]
    public void ArchiveContextFiles_MovesAllContextFiles()
    {
        string contextDir = Path.Combine(_root, "context");
        string archiveDir = Path.Combine(_root, "archive");
        Directory.CreateDirectory(contextDir);

        File.WriteAllText(Path.Combine(contextDir, "current-task.md"), "task");
        File.WriteAllText(Path.Combine(contextDir, "implementation-notes.md"), "notes");
        File.WriteAllText(Path.Combine(contextDir, "review.md"), "review");

        _sut.ArchiveContextFiles(contextDir, archiveDir);

        Assert.True(File.Exists(Path.Combine(archiveDir, "current-task.md")));
        Assert.True(File.Exists(Path.Combine(archiveDir, "implementation-notes.md")));
        Assert.True(File.Exists(Path.Combine(archiveDir, "review.md")));

        Assert.False(File.Exists(Path.Combine(contextDir, "current-task.md")));
        Assert.False(File.Exists(Path.Combine(contextDir, "implementation-notes.md")));
        Assert.False(File.Exists(Path.Combine(contextDir, "review.md")));
    }

    /// <summary>
    /// ArchiveContextFiles skips missing files without throwing.
    /// </summary>
    [Fact]
    public void ArchiveContextFiles_SkipsMissingFilesWithoutThrowing()
    {
        string contextDir = Path.Combine(_root, "context2");
        string archiveDir = Path.Combine(_root, "archive2");
        Directory.CreateDirectory(contextDir);

        // Only write one of the three files.
        File.WriteAllText(Path.Combine(contextDir, "current-task.md"), "task");

        // Should not throw even though implementation-notes.md and review.md are missing.
        _sut.ArchiveContextFiles(contextDir, archiveDir);

        Assert.True(File.Exists(Path.Combine(archiveDir, "current-task.md")));
        Assert.False(File.Exists(Path.Combine(archiveDir, "implementation-notes.md")));
        Assert.False(File.Exists(Path.Combine(archiveDir, "review.md")));
    }

    /// <summary>
    /// ArchiveContextFiles throws DirectoryNotFoundException when the context directory does not exist.
    /// </summary>
    [Fact]
    public void ArchiveContextFiles_ThrowsWhenContextDirDoesNotExist()
    {
        string contextDir = Path.Combine(_root, "nonexistent-context");
        string archiveDir = Path.Combine(_root, "archive3");

        Assert.Throws<DirectoryNotFoundException>(() => _sut.ArchiveContextFiles(contextDir, archiveDir));
    }
}
