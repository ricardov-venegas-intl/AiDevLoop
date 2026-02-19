using System.IO;
using System.Linq;
using System.Text;

namespace AiDevLoop.Shell.Adapters;

/// <summary>
/// Implements <see cref="IFileOperations"/> using the real file system.
/// Write operations are atomic: content is written to a temporary file in the
/// same directory as the target and then moved over the target in one step.
/// </summary>
public sealed class FileOperations : IFileOperations
{
    private static readonly string[] s_contextFileNames =
    [
        "current-task.md",
        "implementation-notes.md",
        "review.md",
    ];

    /// <inheritdoc/>
    public string ReadFile(string filePath)
        => File.ReadAllText(filePath, Encoding.UTF8);

    /// <inheritdoc/>
    public void WriteFile(string filePath, string content)
    {
        string dir = Path.GetDirectoryName(filePath)
            ?? throw new IOException($"Cannot determine parent directory of '{filePath}'.");

        Directory.CreateDirectory(dir);

        string tempPath = Path.Combine(dir, Path.GetRandomFileName());
        try
        {
            File.WriteAllText(tempPath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            File.Move(tempPath, filePath, overwrite: true);
        }
        catch
        {
            // Best-effort cleanup of leftover temp file.
            try { File.Delete(tempPath); } catch { /* ignore */ }
            throw;
        }
    }

    /// <inheritdoc/>
    public void CopyFile(string source, string destination)
    {
        EnsureParentExists(destination);
        File.Copy(source, destination, overwrite: true);
    }

    /// <inheritdoc/>
    public void MoveFile(string source, string destination)
    {
        EnsureParentExists(destination);
        File.Move(source, destination, overwrite: true);
    }

    /// <inheritdoc/>
    public void CreateDirectory(string dirPath)
        => Directory.CreateDirectory(dirPath);

    /// <inheritdoc/>
    public bool FileExists(string filePath)
        => File.Exists(filePath);

    /// <inheritdoc/>
    public bool DirectoryExists(string dirPath)
        => Directory.Exists(dirPath);

    /// <inheritdoc/>
    public IReadOnlyList<string> ListFiles(string dirPath, string? pattern = null)
        => Directory.GetFiles(dirPath, pattern ?? "*", SearchOption.TopDirectoryOnly)
                    .Select(p => Path.GetRelativePath(dirPath, p))
                    .ToArray();

    /// <inheritdoc/>
    public void ArchiveContextFiles(string contextDir, string archiveDir)
    {
        if (!Directory.Exists(contextDir))
            throw new DirectoryNotFoundException($"Context directory not found: '{contextDir}'.");

        Directory.CreateDirectory(archiveDir);

        foreach (string fileName in s_contextFileNames)
        {
            string src = Path.Combine(contextDir, fileName);
            if (!File.Exists(src))
            {
                continue;
            }

            string dest = Path.Combine(archiveDir, fileName);
            File.Move(src, dest, overwrite: true);
        }
    }

    // ── helpers ────────────────────────────────────────────────────────────

    private static void EnsureParentExists(string filePath)
    {
        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}
