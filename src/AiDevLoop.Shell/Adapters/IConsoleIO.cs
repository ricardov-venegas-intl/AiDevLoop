namespace AiDevLoop.Shell.Adapters;

/// <summary>
/// Provides console input and output operations for user interaction.
/// </summary>
public interface IConsoleIO
{
    /// <summary>
    /// Writes a formatted step message to the console.
    /// </summary>
    /// <param name="message">The message to write.</param>
    void WriteStep(string message);

    /// <summary>
    /// Writes an error message to the console.
    /// </summary>
    /// <param name="message">The error message to write.</param>
    void WriteError(string message);

    /// <summary>
    /// Writes a warning message to the console.
    /// </summary>
    /// <param name="message">The warning message to write.</param>
    void WriteWarning(string message);

    /// <summary>
    /// Writes a verbose diagnostic message to the console (only displayed in verbose mode).
    /// </summary>
    /// <param name="message">The verbose message to write.</param>
    void WriteVerbose(string message);

    /// <summary>
    /// Prompts the user with a yes/no question and returns their response.
    /// </summary>
    /// <param name="question">The question to ask the user.</param>
    /// <returns><see langword="true"/> if the user answered yes; <see langword="false"/> if they answered no.</returns>
    bool Confirm(string question);

    /// <summary>
    /// Prompts the user to choose from a list of options and returns the selected value.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with each option.</typeparam>
    /// <param name="question">The question to ask the user.</param>
    /// <param name="options">A read-only list of tuples containing display labels and their associated values.</param>
    /// <returns>The value associated with the user's selected option.</returns>
    T PromptChoice<T>(string question, IReadOnlyList<(string Label, T Value)> options);
}
