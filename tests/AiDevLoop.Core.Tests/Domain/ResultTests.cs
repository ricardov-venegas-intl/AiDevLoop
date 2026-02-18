using AiDevLoop.Core.Domain;
using Xunit;

namespace AiDevLoop.Core.Tests.Domain;

/// <summary>
/// Unit tests for the <see cref="Result{TValue,TError}"/> discriminated union and <see cref="TaskId"/> value object.
/// </summary>
public class ResultTests
{
    // -----------------------------------------------------------------------
    // Result<TValue, TError> — pattern matching
    // -----------------------------------------------------------------------

    /// <summary>
    /// An <see cref="Result{TValue,TError}.Ok"/> result can be matched via a switch expression.
    /// </summary>
    [Fact]
    public void Ok_result_matches_Ok_arm_in_switch_expression()
    {
        Result<int, string> result = new Result<int, string>.Ok(42);

        var output = result switch
        {
            Result<int, string>.Ok ok => ok.Value,
            Result<int, string>.Err err => -1,
            _ => throw new InvalidOperationException("Unexpected case"),
        };

        Assert.Equal(42, output);
    }

    /// <summary>
    /// An <see cref="Result{TValue,TError}.Err"/> result can be matched via a switch expression.
    /// </summary>
    [Fact]
    public void Err_result_matches_Err_arm_in_switch_expression()
    {
        Result<int, string> result = new Result<int, string>.Err("something went wrong");

        var output = result switch
        {
            Result<int, string>.Ok ok => "ok",
            Result<int, string>.Err err => err.Error,
            _ => throw new InvalidOperationException("Unexpected case"),
        };

        Assert.Equal("something went wrong", output);
    }

    /// <summary>
    /// Two <see cref="Result{TValue,TError}.Ok"/> results with the same value are equal.
    /// </summary>
    [Fact]
    public void Ok_results_with_same_value_are_equal()
    {
        var a = new Result<int, string>.Ok(10);
        var b = new Result<int, string>.Ok(10);

        Assert.Equal(a, b);
    }

    /// <summary>
    /// Two <see cref="Result{TValue,TError}.Ok"/> results with different values are not equal.
    /// </summary>
    [Fact]
    public void Ok_results_with_different_values_are_not_equal()
    {
        var a = new Result<int, string>.Ok(10);
        var b = new Result<int, string>.Ok(20);

        Assert.NotEqual(a, b);
    }

    /// <summary>
    /// Two <see cref="Result{TValue,TError}.Err"/> results with the same error are equal.
    /// </summary>
    [Fact]
    public void Err_results_with_same_error_are_equal()
    {
        var a = new Result<int, string>.Err("error");
        var b = new Result<int, string>.Err("error");

        Assert.Equal(a, b);
    }

    /// <summary>
    /// An <see cref="Result{TValue,TError}.Ok"/> result is not equal to an <see cref="Result{TValue,TError}.Err"/> result.
    /// </summary>
    [Fact]
    public void Ok_and_Err_are_not_equal()
    {
        Result<int, string> ok = new Result<int, string>.Ok(0);
        Result<int, string> err = new Result<int, string>.Err("error");

        Assert.NotEqual(ok, err);
    }

    // -----------------------------------------------------------------------
    // TaskId — value object equality
    // -----------------------------------------------------------------------

    /// <summary>
    /// Two <see cref="TaskId"/> instances with the same value are equal.
    /// </summary>
    [Fact]
    public void TaskId_instances_with_same_value_are_equal()
    {
        var a = new TaskId("TASK-001");
        var b = new TaskId("TASK-001");

        Assert.Equal(a, b);
    }

    /// <summary>
    /// Two <see cref="TaskId"/> instances with different values are not equal.
    /// </summary>
    [Fact]
    public void TaskId_instances_with_different_values_are_not_equal()
    {
        var a = new TaskId("TASK-001");
        var b = new TaskId("TASK-002");

        Assert.NotEqual(a, b);
    }

    /// <summary>
    /// <see cref="TaskId.ToString"/> returns the underlying string value.
    /// </summary>
    [Fact]
    public void TaskId_ToString_returns_value()
    {
        var id = new TaskId("TASK-042");

        Assert.Equal("TASK-042", id.ToString());
    }

    /// <summary>
    /// <see cref="TaskId"/> can be used as a dictionary key relying on value equality.
    /// </summary>
    [Fact]
    public void TaskId_can_be_used_as_dictionary_key()
    {
        var dict = new Dictionary<TaskId, string>
        {
            [new TaskId("TASK-001")] = "first",
        };

        Assert.True(dict.ContainsKey(new TaskId("TASK-001")));
        Assert.False(dict.ContainsKey(new TaskId("TASK-999")));
    }
}
