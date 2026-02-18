namespace AiDevLoop.Core.Domain;

/// <summary>
/// Discriminated union representing all possible task-selection failures.
/// </summary>
public abstract record SelectionError;

/// <summary>
/// The requested task could not be found in the implementation plan.
/// </summary>
/// <param name="Id">The identifier that was not found.</param>
public sealed record TaskNotFound(TaskId Id) : SelectionError;

/// <summary>
/// The requested task cannot be started because one or more dependencies are not yet complete.
/// </summary>
/// <param name="Id">The identifier of the task that has unsatisfied dependencies.</param>
/// <param name="Unsatisfied">The identifiers of all dependencies that are not yet done.</param>
public sealed record DependenciesNotMet(TaskId Id, IReadOnlyList<TaskId> Unsatisfied) : SelectionError;

/// <summary>
/// The requested task exists but is not in <see cref="TaskStatus.Pending"/> status.
/// </summary>
/// <param name="Id">The identifier of the task.</param>
/// <param name="CurrentStatus">The actual current status of the task.</param>
public sealed record TaskNotPending(TaskId Id, TaskStatus CurrentStatus) : SelectionError;

/// <summary>
/// No pending tasks are available in the implementation plan.
/// </summary>
public sealed record NoPendingTasks : SelectionError;
