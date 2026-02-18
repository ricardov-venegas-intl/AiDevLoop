namespace AiDevLoop.Core.Domain;

/// <summary>
/// Represents a single task in the implementation plan, including its definition and current status.
/// </summary>
/// <param name="Id">The unique identifier of the task (e.g., <c>TASK-002</c>).</param>
/// <param name="Name">The human-readable name of the task.</param>
/// <param name="Status">The current lifecycle status of the task.</param>
/// <param name="Complexity">The relative complexity of the task.</param>
/// <param name="DependsOn">The identifiers of tasks that must be completed before this task can begin.</param>
/// <param name="Description">A description of what the task requires building.</param>
/// <param name="Steps">Ordered implementation steps for completing the task.</param>
/// <param name="AcceptanceCriteria">Conditions that must be met for the task to be considered done.</param>
/// <param name="FilesInScope">Files that will be created, modified, or referenced by this task.</param>
public record TaskDefinition(
    TaskId Id,
    string Name,
    TaskStatus Status,
    Complexity Complexity,
    IReadOnlyList<TaskId> DependsOn,
    string Description,
    IReadOnlyList<string> Steps,
    IReadOnlyList<string> AcceptanceCriteria,
    IReadOnlyList<FileReference> FilesInScope);
