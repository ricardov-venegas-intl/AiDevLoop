namespace AiDevLoop.Shell.Adapters;

/// <summary>
/// Provides access to an LLM (Large Language Model) for generating responses based on prompts.
/// </summary>
public interface ILLMClient
{
    /// <summary>
    /// Invokes the LLM with the specified prompt and returns the response text.
    /// </summary>
    /// <param name="prompt">The prompt to send to the LLM.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The text response from the LLM.</returns>
    Task<string> InvokeAsync(string prompt, CancellationToken cancellationToken);
}
