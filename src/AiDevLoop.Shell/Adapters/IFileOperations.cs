namespace AiDevLoop.Shell.Adapters;

/// <summary>
/// Provides synchronous file system operations.
/// </summary>
public interface IFileOperations
{
    /// <summary>
    /// Reads the entire content of a file as a string.
    /// </summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <returns>The file content as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs while reading the file.</exception>
    string ReadFile(string filePath);

    /// <summary>
    /// Writes content to a file atomically by writing to a temporary file and then moving it.
    /// </summary>
    /// <param name="filePath">The path to the file to write.</param>
    /// <param name="content">The content to write to the file.</param>
    /// <exception cref="IOException">Thrown when an I/O error occurs while writing the file.</exception>
    void WriteFile(string filePath, string content);

    /// <summary>
    /// Copies a file from the source path to the destination path.
    /// </summary>
    /// <param name="source">The source file path.</param>
    /// <param name="destination">The destination file path.</param>
    /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs while copying the file.</exception>
    void CopyFile(string source, string destination);

    /// <summary>
    /// Moves a file from the source path to the destination path.
    /// </summary>
    /// <param name="source">The source file path.</param>
    /// <param name="destination">The destination file path.</param>
    /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs while moving the file.</exception>
    void MoveFile(string source, string destination);

    /// <summary>
    /// Creates a directory at the specified path, including any necessary parent directories.
    /// </summary>
    /// <param name="dirPath">The path of the directory to create.</param>
    /// <exception cref="IOException">Thrown when an I/O error occurs while creating the directory.</exception>
    void CreateDirectory(string dirPath);

    /// <summary>
    /// Determines whether a file exists at the specified path.
    /// </summary>
    /// <param name="filePath">The path to check.</param>
    /// <returns><see langword="true"/> if the file exists; otherwise, <see langword="false"/>.</returns>
    bool FileExists(string filePath);

    /// <summary>
    /// Determines whether a directory exists at the specified path.
    /// </summary>
    /// <param name="dirPath">The path to check.</param>
    /// <returns><see langword="true"/> if the directory exists; otherwise, <see langword="false"/>.</returns>
    bool DirectoryExists(string dirPath);

    /// <summary>
    /// Lists all files in the specified directory, optionally filtering by a pattern.
    /// </summary>
    /// <param name="dirPath">The directory path to list.</param>
    /// <param name="pattern">An optional search pattern (e.g., <c>"*.md"</c>). If <see langword="null"/>, all files are returned.</param>
    /// <returns>A read-only list of relative file paths in the directory, not including subdirectories.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs while listing files.</exception>
    IReadOnlyList<string> ListFiles(string dirPath, string? pattern = null);

    /// <summary>
    /// Archives context files by moving them from the context directory to the archive directory.
    /// </summary>
    /// <param name="contextDir">The source context directory.</param>
    /// <param name="archiveDir">The destination archive directory.</param>
    /// <exception cref="DirectoryNotFoundException">Thrown when the context directory does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs while archiving files.</exception>
    void ArchiveContextFiles(string contextDir, string archiveDir);
}
