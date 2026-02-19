using System;
using System.Collections.Generic;
using AiDevLoop.Core;
using Xunit;

namespace AiDevLoop.Core.Tests;

public sealed class PromptBuilderTests
{
    // ── Null-guard tests ──────────────────────────────────────────────────────

    [Fact]
    public void BuildPrompt_NullLoadedFiles_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PromptBuilder.BuildPrompt("template", "task", null!, []));
    }

    [Fact]
    public void BuildPrompt_NullContextReferences_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PromptBuilder.BuildPrompt("template", "task", new Dictionary<string, string>(), null!));
    }

    // ── Template + task only ─────────────────────────────────────────────────

    [Fact]
    public void BuildPrompt_EmptyContextReferences_ReturnsTemplateAndTaskOnly()
    {
        string result = PromptBuilder.BuildPrompt(
            "My template",
            "My task",
            new Dictionary<string, string>(),
            []);

        Assert.Equal("My template\n\n---\n\n## Task\n\nMy task", result);
    }

    [Fact]
    public void BuildPrompt_NullTemplate_TreatedAsEmpty()
    {
        string result = PromptBuilder.BuildPrompt(
            null,
            "My task",
            new Dictionary<string, string>(),
            []);

        // No leading separator when template is absent
        Assert.Equal("## Task\n\nMy task", result);
    }

    [Fact]
    public void BuildPrompt_NullTaskContent_TreatedAsEmpty()
    {
        string result = PromptBuilder.BuildPrompt(
            "My template",
            null,
            new Dictionary<string, string>(),
            []);

        Assert.Equal("My template", result);
    }

    [Fact]
    public void BuildPrompt_BothNullOrEmpty_ReturnsEmptyString()
    {
        string result = PromptBuilder.BuildPrompt(
            null,
            null,
            new Dictionary<string, string>(),
            []);

        Assert.Equal(string.Empty, result);
    }

    // ── Context references ────────────────────────────────────────────────────

    [Fact]
    public void BuildPrompt_LoadedFilePresent_AppendsSection()
    {
        var files = new Dictionary<string, string>
        {
            ["docs/architecture.md"] = "Architecture content here."
        };

        string result = PromptBuilder.BuildPrompt(
            "Template",
            "Task",
            files,
            ["docs/architecture.md"]);

        Assert.Contains("\n\n---\n\n## docs/architecture.md\n\nArchitecture content here.", result);
    }

    [Fact]
    public void BuildPrompt_MissingFile_AppendsPlaceholder()
    {
        string result = PromptBuilder.BuildPrompt(
            "Template",
            "Task",
            new Dictionary<string, string>(),
            ["docs/missing.md"]);

        Assert.Contains("\n\n---\n\n<!-- docs/missing.md not found -->", result);
    }

    [Fact]
    public void BuildPrompt_MixedPresentAndMissingFiles_HandlesEach()
    {
        var files = new Dictionary<string, string>
        {
            ["docs/present.md"] = "Present content."
        };

        string result = PromptBuilder.BuildPrompt(
            "Template",
            "Task",
            files,
            ["docs/present.md", "docs/absent.md"]);

        Assert.Contains("\n\n---\n\n## docs/present.md\n\nPresent content.", result);
        Assert.Contains("\n\n---\n\n<!-- docs/absent.md not found -->", result);
    }

    [Fact]
    public void BuildPrompt_ConcatenationOrder_IsTemplateTaskThenRefs()
    {
        var files = new Dictionary<string, string>
        {
            ["docs/ref.md"] = "Ref content."
        };

        string result = PromptBuilder.BuildPrompt(
            "TEMPLATE",
            "TASK",
            files,
            ["docs/ref.md"]);

        int templatePos = result.IndexOf("TEMPLATE", StringComparison.Ordinal);
        int taskPos = result.IndexOf("TASK", StringComparison.Ordinal);
        int refPos = result.IndexOf("Ref content.", StringComparison.Ordinal);

        Assert.True(templatePos < taskPos, "Template must appear before task.");
        Assert.True(taskPos < refPos, "Task must appear before reference content.");
    }

    [Fact]
    public void BuildPrompt_MultipleRefsPreserveOrder()
    {
        var files = new Dictionary<string, string>
        {
            ["docs/first.md"] = "First.",
            ["docs/second.md"] = "Second."
        };

        string result = PromptBuilder.BuildPrompt(
            "T",
            "T",
            files,
            ["docs/first.md", "docs/second.md"]);

        int firstPos = result.IndexOf("First.", StringComparison.Ordinal);
        int secondPos = result.IndexOf("Second.", StringComparison.Ordinal);

        Assert.True(firstPos < secondPos, "First ref must appear before second ref.");
    }

    [Fact]
    public void BuildPrompt_OnlyContextReferencesNoTemplateNoTask_BuildsJustRefs()
    {
        var files = new Dictionary<string, string>
        {
            ["docs/arch.md"] = "Arch."
        };

        string result = PromptBuilder.BuildPrompt(
            null,
            null,
            files,
            ["docs/arch.md"]);

        Assert.Equal("## docs/arch.md\n\nArch.", result);
    }

    // ── Extra coverage: ignored files and empty-string equivalence ────────────

    [Fact]
    public void BuildPrompt_LoadedFilesNotInContextReferences_AreIgnored()
    {
        var files = new Dictionary<string, string>
        {
            ["docs/referenced.md"] = "Referenced.",
            ["docs/unreferenced.md"] = "Should not appear."
        };

        string result = PromptBuilder.BuildPrompt(
            "Template",
            "Task",
            files,
            ["docs/referenced.md"]);

        Assert.Contains("Referenced.", result);
        Assert.DoesNotContain("Should not appear.", result);
    }

    [Fact]
    public void BuildPrompt_EmptyStringTemplate_TreatedSameAsNull()
    {
        string withNull = PromptBuilder.BuildPrompt(null, "My task", new Dictionary<string, string>(), []);
        string withEmpty = PromptBuilder.BuildPrompt("", "My task", new Dictionary<string, string>(), []);

        Assert.Equal(withNull, withEmpty);
    }

    [Fact]
    public void BuildPrompt_EmptyStringTaskContent_TreatedSameAsNull()
    {
        string withNull = PromptBuilder.BuildPrompt("My template", null, new Dictionary<string, string>(), []);
        string withEmpty = PromptBuilder.BuildPrompt("My template", "", new Dictionary<string, string>(), []);

        Assert.Equal(withNull, withEmpty);
    }
}
