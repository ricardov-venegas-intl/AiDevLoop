namespace AiDevLoop.Core.Domain;

/// <summary>
/// Controls the verbosity level of console output.
/// </summary>
public enum OutputMode
{
    /// <summary>
    /// Default output level: shows step, error, and warning messages.
    /// </summary>
    Normal,

    /// <summary>
    /// All output including verbose diagnostic messages.
    /// </summary>
    Verbose,

    /// <summary>
    /// Minimal output: only errors and warnings are shown.
    /// </summary>
    Quiet,
}
