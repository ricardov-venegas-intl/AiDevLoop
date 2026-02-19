namespace AiDevLoop.Shell.Adapters;

using System;
using System.Threading;
using System.Threading.Tasks;

using AiDevLoop.Core.Domain;

/// <summary>
/// Implements <see cref="ILLMClient"/> by invoking the GitHub Copilot CLI tool.
/// </summary>
/// <remarks>
/// Invokes <c>copilot -p "&lt;prompt&gt;"</c> via the injected <see cref="IProcessRunner"/>.
/// Inner double-quotes in the prompt are escaped before being passed on the command line.
/// </remarks>
public sealed class CopilotLLMClient : ILLMClient
{
    private readonly IProcessRunner _processRunner;

    /// <summary>
    /// Initialises a new instance of <see cref="CopilotLLMClient"/>.
    /// </summary>
    /// <param name="processRunner">The process runner used to invoke the Copilot CLI.</param>
    public CopilotLLMClient(IProcessRunner processRunner)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    /// <inheritdoc/>
    public async Task<string> InvokeAsync(string prompt, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(prompt);

        string escapedPrompt = prompt.Replace("\"", "\\\"");
        string arguments = $"-p \"{escapedPrompt}\"";

        CommandResult result = await _processRunner
            .RunAsync("copilot", arguments, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Copilot CLI exited with code {result.ExitCode}. Stderr: {result.Stderr}. Stdout: {result.Stdout}");
        }

        string response = result.Stdout.Trim();

        if (string.IsNullOrWhiteSpace(response))
        {
            throw new InvalidOperationException("Copilot CLI returned an empty response.");
        }

        return response;
    }
}
