using System;
using System.Collections.Generic;
using System.IO;

using AiDevLoop.Core.Domain;

namespace AiDevLoop.Shell.Adapters;

/// <summary>
/// Concrete implementation of <see cref="IConsoleIO"/> that respects <see cref="OutputMode"/>
/// and optionally applies color when writing to the real console.
/// </summary>
public sealed class ConsoleIO : IConsoleIO
{
    private readonly OutputMode _mode;
    private readonly TextWriter _output;
    private readonly TextReader _input;
    private readonly bool _useColor;

    /// <summary>
    /// Initializes a new instance of <see cref="ConsoleIO"/> with injected I/O streams.
    /// Color output is disabled when using injected streams.
    /// </summary>
    /// <param name="mode">The output verbosity mode.</param>
    /// <param name="output">The writer to use for output.</param>
    /// <param name="input">The reader to use for input.</param>
    public ConsoleIO(OutputMode mode, TextWriter output, TextReader input)
    {
        _mode = mode;
        _output = output;
        _input = input;
        _useColor = false;
    }

    private ConsoleIO(OutputMode mode, TextWriter output, TextReader input, bool useColor)
    {
        _mode = mode;
        _output = output;
        _input = input;
        _useColor = useColor;
    }

    /// <summary>
    /// Creates a <see cref="ConsoleIO"/> instance that writes to and reads from the real console.
    /// Color output is enabled when the output is not redirected.
    /// </summary>
    /// <param name="mode">The output verbosity mode.</param>
    /// <returns>A configured <see cref="ConsoleIO"/> instance.</returns>
    public static ConsoleIO CreateForConsole(OutputMode mode) =>
        new(mode, Console.Out, Console.In, useColor: !Console.IsOutputRedirected);

    /// <inheritdoc />
    public void WriteStep(string message)
    {
        if (_mode == OutputMode.Quiet)
        {
            return;
        }

        _output.WriteLine(message);
    }

    /// <inheritdoc />
    public void WriteError(string message)
    {
        if (_useColor)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        _output.WriteLine(message);

        if (_useColor)
        {
            Console.ResetColor();
        }
    }

    /// <inheritdoc />
    public void WriteWarning(string message)
    {
        if (_useColor)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        _output.WriteLine(message);

        if (_useColor)
        {
            Console.ResetColor();
        }
    }

    /// <inheritdoc />
    public void WriteVerbose(string message)
    {
        if (_mode != OutputMode.Verbose)
        {
            return;
        }

        if (_useColor)
        {
            // Dim via dark gray approximation
            Console.ForegroundColor = ConsoleColor.DarkGray;
            _output.WriteLine(message);
            Console.ResetColor();
        }
        else
        {
            _output.WriteLine($"[VERBOSE] {message}");
        }
    }

    /// <inheritdoc />
    public bool Confirm(string question)
    {
        _output.Write($"{question} [y/N]: ");
        string? line = _input.ReadLine();
        return string.Equals(line, "y", StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public T PromptChoice<T>(string question, IReadOnlyList<(string Label, T Value)> options)
    {
        _output.WriteLine(question);

        for (int i = 0; i < options.Count; i++)
        {
            _output.WriteLine($"{i + 1}. {options[i].Label}");
        }

        while (true)
        {
            _output.Write("Enter choice: ");
            string? line = _input.ReadLine();

            if (int.TryParse(line, out int choice) && choice >= 1 && choice <= options.Count)
            {
                return options[choice - 1].Value;
            }

            _output.WriteLine($"Invalid input. Please enter a number between 1 and {options.Count}.");
        }
    }
}
