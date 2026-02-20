namespace AiDevLoop.Shell.Adapters;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AiDevLoop.Core.Domain;

/// <summary>
/// Runs external processes and captures their standard output and standard error.
/// </summary>
public sealed class ProcessRunner : IProcessRunner
{
    private const int NonVerboseQueueLimit = 500;

    /// <inheritdoc/>
    public Task<CommandResult> RunAsync(
        string command,
        string arguments,
        CancellationToken cancellationToken) =>
        RunAsync(command, arguments, workingDirectory: string.Empty, verbose: false, cancellationToken);

    /// <inheritdoc/>
    public async Task<CommandResult> RunAsync(
        string command,
        string arguments,
        string workingDirectory,
        bool verbose,
        CancellationToken cancellationToken)
    {
        string resolvedDirectory = string.IsNullOrEmpty(workingDirectory)
            ? Directory.GetCurrentDirectory()
            : workingDirectory;

        var startInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = resolvedDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        List<string>? stdoutLines = verbose ? [] : null;
        List<string>? stderrLines = verbose ? [] : null;
        Queue<string>? stdoutQueue = verbose ? null : new();
        Queue<string>? stderrQueue = verbose ? null : new();

        using var process = new Process { StartInfo = startInfo };

        if (verbose)
        {
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    lock (stdoutLines!)
                        stdoutLines.Add(e.Data);
                }
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    lock (stderrLines!)
                        stderrLines.Add(e.Data);
                }
            };
        }
        else
        {
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    lock (stdoutQueue!)
                    {
                        stdoutQueue.Enqueue(e.Data);
                        if (stdoutQueue.Count > NonVerboseQueueLimit)
                            stdoutQueue.Dequeue();
                    }
                }
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    lock (stderrQueue!)
                    {
                        stderrQueue.Enqueue(e.Data);
                        if (stderrQueue.Count > NonVerboseQueueLimit)
                            stderrQueue.Dequeue();
                    }
                }
            };
        }

        try
        {
            process.Start();
        }
        catch (Win32Exception)
        {
            return new CommandResult(command, -1, string.Empty, $"Command not found: {command}");
        }
        catch (FileNotFoundException)
        {
            return new CommandResult(command, -1, string.Empty, $"Command not found: {command}");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }

        string stdout;
        string stderr;

        if (verbose)
        {
            lock (stdoutLines!)
                stdout = string.Join(Environment.NewLine, stdoutLines);
            lock (stderrLines!)
                stderr = string.Join(Environment.NewLine, stderrLines);
        }
        else
        {
            lock (stdoutQueue!)
                stdout = string.Join(Environment.NewLine, stdoutQueue);
            lock (stderrQueue!)
                stderr = string.Join(Environment.NewLine, stderrQueue);
        }

        return new CommandResult(command, process.ExitCode, stdout, stderr);
    }
}
