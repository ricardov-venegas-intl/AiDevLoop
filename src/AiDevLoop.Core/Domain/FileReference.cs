namespace AiDevLoop.Core.Domain;

/// <summary>
/// A reference to a file included in a task's scope, along with its intended usage.
/// </summary>
/// <param name="Path">The relative path to the file.</param>
/// <param name="Kind">Whether the file is to be created, modified, or used as a read-only reference.</param>
public record FileReference(string Path, FileReferenceKind Kind);
