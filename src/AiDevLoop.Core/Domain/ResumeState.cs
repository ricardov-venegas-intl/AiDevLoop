namespace AiDevLoop.Core.Domain;

/// <summary>
/// Persisted state that allows the development loop to resume an interrupted task.
/// </summary>
/// <param name="NextStep">The one-based index of the next step to execute when resuming.</param>
/// <param name="TaskId">The identifier of the task being resumed.</param>
public record ResumeState(int NextStep, TaskId TaskId);
