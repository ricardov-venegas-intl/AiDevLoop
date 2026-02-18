namespace AiDevLoop.Core.Domain;

/// <summary>
/// Describes how a file is related to a task definition.
/// </summary>
public enum FileReferenceKind
{
    /// <summary>The file will be created as part of this task.</summary>
    Create,

    /// <summary>The file will be modified as part of this task.</summary>
    Modify,

    /// <summary>The file is read for context but will not be changed by this task.</summary>
    ReadOnlyReference,
}
