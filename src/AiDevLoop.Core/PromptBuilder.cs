using System;
using System.Collections.Generic;
using System.Text;

namespace AiDevLoop.Core;

/// <summary>
/// Constructs LLM agent prompts by combining a prompt template, task content,
/// and pre-loaded reference file contents into a single concatenated string.
/// </summary>
public static class PromptBuilder
{
    /// <summary>
    /// Builds a prompt string by concatenating the prompt template, task content,
    /// and any referenced files in order. Sections are separated by horizontal rules
    /// with a header identifying the source key.
    /// </summary>
    /// <param name="promptTemplate">
    /// The base prompt template text. Treated as empty string if null or empty.
    /// </param>
    /// <param name="taskContent">
    /// The task definition content. Treated as empty string if null or empty.
    /// </param>
    /// <param name="loadedFiles">
    /// A dictionary mapping context reference keys to their pre-loaded file contents.
    /// Must not be null.
    /// </param>
    /// <param name="contextReferences">
    /// An ordered list of context reference keys to include. Must not be null.
    /// Only these keys are appended; keys absent from <paramref name="loadedFiles"/>
    /// produce a placeholder comment.
    /// </param>
    /// <returns>
    /// A single concatenated string suitable for passing to an LLM CLI tool.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="loadedFiles"/> or <paramref name="contextReferences"/> is null.
    /// </exception>
    public static string BuildPrompt(
        string? promptTemplate,
        string? taskContent,
        IReadOnlyDictionary<string, string> loadedFiles,
        IReadOnlyList<string> contextReferences)
    {
        ArgumentNullException.ThrowIfNull(loadedFiles);
        ArgumentNullException.ThrowIfNull(contextReferences);

        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(promptTemplate))
        {
            sb.Append(promptTemplate);
        }

        if (!string.IsNullOrEmpty(taskContent))
        {
            if (sb.Length > 0)
            {
                sb.Append("\n\n---\n\n");
            }

            sb.Append("## Task\n\n");
            sb.Append(taskContent);
        }

        foreach (string key in contextReferences)
        {
            if (loadedFiles.TryGetValue(key, out string? content))
            {
                if (sb.Length > 0)
                {
                    sb.Append("\n\n---\n\n");
                }

                sb.Append("## ");
                sb.Append(key);
                sb.Append("\n\n");
                sb.Append(content);
            }
            else
            {
                if (sb.Length > 0)
                {
                    sb.Append("\n\n---\n\n");
                }

                sb.Append("<!-- ");
                sb.Append(key);
                sb.Append(" not found -->");
            }
        }

        return sb.ToString();
    }
}
