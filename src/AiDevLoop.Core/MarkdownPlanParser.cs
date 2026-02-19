using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AiDevLoop.Core.Domain;

namespace AiDevLoop.Core
{
    /// <summary>
    /// Parses a Markdown implementation plan (the format used in docs/implementation-plan.md)
    /// into a <see cref="Plan"/> value.
    /// </summary>
    public static class MarkdownPlanParser
    {
        /// <summary>
        /// Parse the provided markdown content into a <see cref="Plan"/>.
        /// Returns <see cref="Result{TValue,TError}.Ok"/> on success or
        /// <see cref="Result{TValue,TError}.Err"/> with an error message on failure.
        /// </summary>
        /// <param name="markdownContent">The full markdown text of an implementation plan.</param>
        public static Result<Plan, string> Parse(string markdownContent)
        {
            if (string.IsNullOrWhiteSpace(markdownContent))
                return new Result<Plan, string>.Err("Plan content is empty.");

            // Normalize line endings for simpler indexing
            var normalized = markdownContent.Replace("\r\n", "\n");

            // Title: first '# ' heading
            var titleMatch = Regex.Match(normalized, @"^#\s+(.+)$", RegexOptions.Multiline);
            var title = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : "Implementation Plan";

            // 1) Parse milestone summary lists (top-of-file checklist under each milestone)
            var milestoneHeader = new Regex(@"^##\s+Milestone\s+(\d+)\s+—\s*(.+)$", RegexOptions.Multiline);
            var milestoneMatches = milestoneHeader.Matches(normalized);

            var milestones = new List<Milestone>();

            foreach (Match mh in milestoneMatches)
            {
                var number = int.Parse(mh.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                var name = mh.Groups[2].Value.Trim();

                // Collect lines following this header up to the next '## ' header
                var startIndex = mh.Index + mh.Length;
                var remainder = normalized.Substring(startIndex);
                var nextHeader = Regex.Match(remainder, "^##\\s+", RegexOptions.Multiline);
                var sectionText = nextHeader.Success ? remainder.Substring(0, nextHeader.Index) : remainder;

                // Find task checklist items in this section
                var taskLines = Regex.Matches(sectionText, @"^\s*-\s*\[( |x)\]\s*(TASK-\d+)\s*·\s*([^·]+?)\s*·\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                var tasks = new List<TaskDefinition>();
                foreach (Match tl in taskLines)
                {
                    var checkedBox = tl.Groups[1].Value.Trim();
                    var id = tl.Groups[2].Value.Trim();
                    var complexityStr = tl.Groups[3].Value.Trim();
                    var nameTitle = tl.Groups[4].Value.Trim();

                    var status = checkedBox.Equals("x", StringComparison.OrdinalIgnoreCase) ? AiDevLoop.Core.Domain.TaskStatus.Done : AiDevLoop.Core.Domain.TaskStatus.Pending;
                    _ = Enum.TryParse<Complexity>(complexityStr, true, out var complexity);

                    // Minimal placeholder TaskDefinition; detailed block (if present) will replace this later
                    var td = new TaskDefinition(new TaskId(id), nameTitle, status, complexity, Array.Empty<TaskId>(), string.Empty, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<FileReference>());
                    tasks.Add(td);
                }

                milestones.Add(new Milestone(number, name, tasks));
            }

            // 2) Parse detailed task definition blocks (## TASK-XXX: Title)
            var taskBlockHeader = new Regex(@"^##\s*(TASK-\d+):\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var blockMatches = taskBlockHeader.Matches(normalized);

            var detailed = new Dictionary<string, TaskDefinition>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < blockMatches.Count; i++)
            {
                var m = blockMatches[i];
                var id = m.Groups[1].Value.Trim();
                var taskName = m.Groups[2].Value.Trim();

                var blockStart = m.Index + m.Length;
                var blockEnd = (i + 1) < blockMatches.Count ? blockMatches[i + 1].Index : normalized.Length;
                var blockText = normalized.Substring(blockStart, blockEnd - blockStart).Trim();

                // Extract simple metadata fields
                var statusMatch = Regex.Match(blockText, @"\*\*Status:\*\*\s*(.+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var complexityMatch = Regex.Match(blockText, @"\*\*Complexity:\*\*\s*(.+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var dependsMatch = Regex.Match(blockText, @"\*\*Depends on:\*\*\s*(.+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                var status = AiDevLoop.Core.Domain.TaskStatus.Pending;
                if (statusMatch.Success)
                {
                    var s = statusMatch.Groups[1].Value.Trim().ToLowerInvariant();
                    if (s.Contains("pending")) status = AiDevLoop.Core.Domain.TaskStatus.Pending;
                    else if (s.Contains("done") || s.Contains("completed")) status = AiDevLoop.Core.Domain.TaskStatus.Done;
                    else if (s.Contains("in-progress") || s.Contains("in progress")) status = AiDevLoop.Core.Domain.TaskStatus.InProgress;
                    else if (s.Contains("blocked")) status = AiDevLoop.Core.Domain.TaskStatus.Blocked;
                }

                var complexity = Complexity.Medium;
                if (complexityMatch.Success)
                {
                    _ = Enum.TryParse<Complexity>(complexityMatch.Groups[1].Value.Trim(), true, out complexity);
                }

                IReadOnlyList<TaskId> dependsOn = Array.Empty<TaskId>();
                if (dependsMatch.Success)
                {
                    var d = dependsMatch.Groups[1].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(d) && d != "—" && d != "-")
                    {
                        dependsOn = d.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => new TaskId(s.Trim()))
                                     .ToList();
                    }
                }

                // Sections
                string Section(string name)
                {
                    var secMatch = Regex.Match(blockText, $@"###\s+{Regex.Escape(name)}\s*(\r?\n)", RegexOptions.IgnoreCase);
                    if (!secMatch.Success) return string.Empty;
                    var secIndex = secMatch.Index + secMatch.Length;
                    var after = blockText.Substring(secIndex);
                    var next = Regex.Match(after, "^###\\s+", RegexOptions.Multiline);
                    return next.Success ? after.Substring(0, next.Index).Trim() : after.Trim();
                }

                var whatToBuild = Section("What to build");
                var filesSection = Section("Files in scope");
                var validationSection = Section("Validation criteria");

                var description = string.Empty;
                var steps = new List<string>();
                var acceptance = new List<string>();
                var files = new List<FileReference>();

                if (!string.IsNullOrWhiteSpace(whatToBuild))
                {
                    description = Regex.Replace(whatToBuild.Trim(), @"\r?\n", " ").Trim();
                    var stepMatches = Regex.Matches(whatToBuild, "^\\s*\\d+\\.\\s*(.+)$", RegexOptions.Multiline);
                    foreach (Match sm in stepMatches) steps.Add(sm.Groups[1].Value.Trim());
                }

                if (!string.IsNullOrWhiteSpace(filesSection))
                {
                    var fileLines = filesSection.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var fl in fileLines)
                    {
                        var fm = Regex.Match(fl, "`?(?<path>[^()`]+)`?\\s*\\((?<kind>[^)]+)\\)");
                        if (!fm.Success) continue;
                        var path = fm.Groups["path"].Value.Trim();
                        var kindStr = fm.Groups["kind"].Value.Trim().ToLowerInvariant();
                        var kind = FileReferenceKind.ReadOnlyReference;
                        if (kindStr.Contains("create")) kind = FileReferenceKind.Create;
                        else if (kindStr.Contains("modify")) kind = FileReferenceKind.Modify;
                        else kind = FileReferenceKind.ReadOnlyReference;
                        files.Add(new FileReference(path, kind));
                    }
                }

                if (!string.IsNullOrWhiteSpace(validationSection))
                {
                    var valLines = validationSection.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var vl in valLines)
                    {
                        var m2 = Regex.Match(vl, @"^\s*-\s*\[.\]\s*(.+)$");
                        if (!m2.Success) m2 = Regex.Match(vl, @"^\s*-\s*(.+)$");
                        if (m2.Success) acceptance.Add(m2.Groups[1].Value.Trim());
                    }
                }

                var td = new TaskDefinition(new TaskId(id), taskName, status, complexity, dependsOn, description, steps, acceptance, files);
                detailed[id] = td;
            }

            if (!detailed.Any())
                return new Result<Plan, string>.Err("No task definitions found in the plan.");

            // Replace milestone placeholder tasks with detailed ones when available
            var finalMilestones = milestones.Select(m =>
            {
                var replaced = m.Tasks.Select(t => detailed.TryGetValue(t.Id.Value, out var d) ? d : t).ToList();
                return new Milestone(m.Number, m.Name, replaced);
            }).ToList();

            var plan = new Plan(title, finalMilestones);
            return new Result<Plan, string>.Ok(plan);
        }
    }
}
