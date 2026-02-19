namespace AiDevLoop.Shell.Adapters;

using System.Text;

/// <summary>
/// Utility methods for building safe command-line argument strings.
/// </summary>
internal static class ProcessArgumentHelper
{
    /// <summary>
    /// Wraps <paramref name="value"/> in double-quotes with proper Windows
    /// CommandLineToArgvW-compatible escaping (handles backslashes before quotes and
    /// trailing backslashes that would otherwise escape the closing quote).
    /// </summary>
    /// <param name="value">The raw argument value to escape.</param>
    /// <returns>A quoted, escaped argument string ready for use in a command line.</returns>
    internal static string EscapeArgument(string value)
    {
        var sb = new StringBuilder();
        sb.Append('"');
        int backslashCount = 0;
        foreach (char c in value)
        {
            if (c == '\\')
            {
                backslashCount++;
            }
            else if (c == '"')
            {
                sb.Append('\\', backslashCount * 2 + 1);
                sb.Append('"');
                backslashCount = 0;
            }
            else
            {
                sb.Append('\\', backslashCount);
                sb.Append(c);
                backslashCount = 0;
            }
        }

        // Trailing backslashes must be doubled so they don't escape the closing quote.
        sb.Append('\\', backslashCount * 2);
        sb.Append('"');
        return sb.ToString();
    }
}
