namespace AiDevLoop.Core.Domain;

/// <summary>
/// Discriminated union representing either a successful value or an error.
/// Use pattern matching with <see langword="switch"/> expressions to handle both cases.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public abstract record Result<TValue, TError>
{
    /// <summary>
    /// A successful result containing a value.
    /// </summary>
    /// <param name="Value">The success value.</param>
    public sealed record Ok(TValue Value) : Result<TValue, TError>;

    /// <summary>
    /// A failed result containing an error.
    /// </summary>
    /// <param name="Error">The error value describing the failure.</param>
    public sealed record Err(TError Error) : Result<TValue, TError>;
}
