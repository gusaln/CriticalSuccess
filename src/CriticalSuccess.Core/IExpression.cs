namespace CriticalSuccess.Core;

/// <summary>
/// Represents an expression with a value in a roll.
/// </summary>
/// <remarks>
/// Expressions should be immutable and comparable.
/// </remarks>
public interface IExpression
{
    /// <summary>
    /// Gets a value from the expression.
    /// </summary>
    /// <remarks> Multiple calls to this method could return different values. </remarks>
    /// <returns> The value from the expression. </returns>
    int Value()
    {
        return Result().Value;
    }

    /// <summary>
    /// /// Gets a Result from the expression. Multiple calls to this method could return different Results.
    /// </summary>
    /// <returns> The Result from the expression. </returns>
    Result Result();
}
