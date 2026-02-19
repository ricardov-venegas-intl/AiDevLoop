using AiDevLoop.Core.Domain;

namespace AiDevLoop.Cli;

/// <summary>
/// Parses raw command-line arguments into a <see cref="CommandLineArgs"/> record.
/// </summary>
/// <remarks>
/// Supported syntax:
/// <code>
///   aidevloop run [TASK-ID] [OPTIONS]
///   aidevloop resume [OPTIONS]
/// </code>
/// Options:
/// <list type="bullet">
///   <item><c>--llm=&lt;provider&gt;</c> — override the configured LLM provider.</item>
///   <item><c>--config=&lt;path&gt;</c> — path to an alternate configuration file.</item>
///   <item><c>--verbose</c> — enable verbose output.</item>
///   <item><c>--quiet</c> — suppress step-progress output.</item>
///   <item><c>--from-step=&lt;n&gt;</c> — resume from a specific step (resume only).</item>
/// </list>
/// </remarks>
public static class CommandLineParser
{
    /// <summary>
    /// Parses the supplied argument array into a <see cref="CommandLineArgs"/> value.
    /// </summary>
    /// <param name="args">The raw arguments passed to the executable.</param>
    /// <returns>
    /// <see cref="Result{TValue,TError}.Ok"/> containing the parsed <see cref="CommandLineArgs"/>,
    /// or <see cref="Result{TValue,TError}.Err"/> containing a human-readable error message.
    /// </returns>
    public static Result<CommandLineArgs, string> Parse(string[] args)
    {
        if (args.Length == 0)
            return Error("No command specified. Use 'run' or 'resume'.");

        if (!TryParseCommand(args[0], out var command))
            return Error($"Unknown command '{args[0]}'. Use 'run' or 'resume'.");

        int flagStart = 1;
        TaskId? taskId = null;

        // For 'run', an optional positional task ID may follow the command.
        if (command == Command.Run
            && args.Length > 1
            && !args[1].StartsWith("--", StringComparison.Ordinal))
        {
            taskId = new TaskId(args[1]);
            flagStart = 2;
        }

        string? llm = null;
        string? configPath = null;
        bool verbose = false;
        bool quiet = false;
        int? fromStep = null;

        for (int i = flagStart; i < args.Length; i++)
        {
            var arg = args[i];

            if (TryGetFlagValue(arg, "--llm=", out var llmValue))
            {
                llm = llmValue;
            }
            else if (TryGetFlagValue(arg, "--config=", out var configValue))
            {
                configPath = configValue;
            }
            else if (arg == "--verbose")
            {
                verbose = true;
            }
            else if (arg == "--quiet")
            {
                quiet = true;
            }
            else if (TryGetFlagValue(arg, "--from-step=", out var stepStr))
            {
                if (!int.TryParse(stepStr, out var step) || step < 1)
                    return Error($"Invalid value for --from-step: '{stepStr}'. Must be a positive integer.");

                fromStep = step;
            }
            else
            {
                return Error($"Unknown flag '{arg}'.");
            }
        }

        if (verbose && quiet)
            return Error("--verbose and --quiet cannot be used together.");

        if (fromStep.HasValue && command == Command.Run)
            return Error("--from-step can only be used with the 'resume' command.");

        var options = new CommandLineOptions(llm, configPath, verbose, quiet, fromStep);
        return new Result<CommandLineArgs, string>.Ok(new CommandLineArgs(command, taskId, options));
    }

    private static bool TryParseCommand(string value, out Command command)
    {
        switch (value.ToLowerInvariant())
        {
            case "run":
                command = Command.Run;
                return true;
            case "resume":
                command = Command.Resume;
                return true;
            default:
                command = default;
                return false;
        }
    }

    private static bool TryGetFlagValue(string arg, string prefix, out string value)
    {
        if (arg.StartsWith(prefix, StringComparison.Ordinal))
        {
            value = arg[prefix.Length..];
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static Result<CommandLineArgs, string>.Err Error(string message) =>
        new Result<CommandLineArgs, string>.Err(message);
}
