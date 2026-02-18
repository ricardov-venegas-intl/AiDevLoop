namespace AiDevLoop.Core.Domain;

/// <summary>
/// The top-level CLI command to execute.
/// </summary>
public enum Command
{
    /// <summary>Start execution of the next pending (or specified) task.</summary>
    Run,

    /// <summary>Resume an interrupted task from the last saved step.</summary>
    Resume,
}
