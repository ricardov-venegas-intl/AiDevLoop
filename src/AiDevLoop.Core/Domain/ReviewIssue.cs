namespace AiDevLoop.Core.Domain;

/// <summary>
/// A single issue identified during a code review, along with its severity classification.
/// </summary>
/// <param name="Description">A human-readable description of the issue.</param>
/// <param name="Classification">The severity of the issue.</param>
public record ReviewIssue(string Description, IssueClassification Classification);
