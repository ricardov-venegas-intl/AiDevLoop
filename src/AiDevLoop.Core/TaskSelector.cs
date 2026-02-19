using AiDevLoop.Core.Domain;
using TaskStatus = AiDevLoop.Core.Domain.TaskStatus;

namespace AiDevLoop.Core;

/// <summary>
/// Selects the next task to execute from a <see cref="Plan"/>.
/// </summary>
public static class TaskSelector
{
    /// <summary>
    /// Selects a task to execute from the given plan.
    /// </summary>
    /// <remarks>
    /// When <paramref name="taskId"/> is <see langword="null"/>, iterates milestones and tasks in
    /// their defined order and returns the first <see cref="AiDevLoop.Core.Domain.TaskStatus.Pending"/> task whose every
    /// dependency has status <see cref="AiDevLoop.Core.Domain.TaskStatus.Done"/>. Returns <see cref="NoPendingTasks"/>
    /// when no eligible task exists.
    ///
    /// When <paramref name="taskId"/> is provided, validates that the task exists, is
    /// <see cref="AiDevLoop.Core.Domain.TaskStatus.Pending"/>, and has all dependencies satisfied before returning it.
    /// </remarks>
    /// <param name="plan">The implementation plan to select from.</param>
    /// <param name="taskId">
    /// An explicit task identifier to select, or <see langword="null"/> to auto-select.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue,TError}"/> containing the selected <see cref="TaskDefinition"/>
    /// on success, or a <see cref="SelectionError"/> describing the failure.
    /// </returns>
    public static Result<TaskDefinition, SelectionError> SelectTask(Plan plan, TaskId? taskId)
    {
        var taskMap = new Dictionary<TaskId, TaskDefinition>();

        foreach (var task in plan.Milestones.SelectMany(m => m.Tasks))
        {
            // Last-write-wins for duplicate task IDs to avoid exceptions from ToDictionary.
            taskMap[task.Id] = task;
        }

        return taskId is null
            ? AutoSelect(plan, taskMap)
            : ExplicitSelect(taskId.Value, taskMap);
    }

    private static Result<TaskDefinition, SelectionError> AutoSelect(
        Plan plan,
        Dictionary<TaskId, TaskDefinition> taskMap)
    {
        var candidate = plan.Milestones
            .SelectMany(m => m.Tasks.OrderBy(t => t.Id.Value, StringComparer.Ordinal))
            .FirstOrDefault(t => t.Status == TaskStatus.Pending && AllDepsDone(t, taskMap));

        return candidate is null
            ? new Result<TaskDefinition, SelectionError>.Err(new NoPendingTasks())
            : new Result<TaskDefinition, SelectionError>.Ok(candidate);
    }

    private static Result<TaskDefinition, SelectionError> ExplicitSelect(
        TaskId taskId,
        Dictionary<TaskId, TaskDefinition> taskMap)
    {
        if (!taskMap.TryGetValue(taskId, out var task))
            return new Result<TaskDefinition, SelectionError>.Err(new TaskNotFound(taskId));

        if (task.Status != TaskStatus.Pending)
            return new Result<TaskDefinition, SelectionError>.Err(new TaskNotPending(task.Id, task.Status));

        var unsatisfied = task.DependsOn
            .Where(dep => !taskMap.TryGetValue(dep, out var depTask) || depTask.Status != TaskStatus.Done)
            .ToList();

        return unsatisfied.Count > 0
            ? new Result<TaskDefinition, SelectionError>.Err(new DependenciesNotMet(task.Id, unsatisfied))
            : new Result<TaskDefinition, SelectionError>.Ok(task);
    }

    private static bool AllDepsDone(TaskDefinition task, Dictionary<TaskId, TaskDefinition> taskMap) =>
        task.DependsOn.All(dep => taskMap.TryGetValue(dep, out var depTask) && depTask.Status == TaskStatus.Done);
}
