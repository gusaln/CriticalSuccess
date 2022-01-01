namespace CriticalSuccess.Core;

/// <summary>
/// Represents a constant value in a roll.
/// </summary>
public record Constant : IExpression
{
    /// <summary> Stores the value of the constant. </summary>
    public readonly int Number;

    /// <summary>
    /// Creates and expression with a constant value
    /// </summary>
    /// <param name="number">The value of the constant</param>
    public Constant(int number)
    {
        Number = number;
    }

    /// <summary>
    /// Returns the value of the constant.
    /// </summary>
    /// <returns>The value of the constant wrapped in a <c>Result</c></returns>
    public Result Result()
    {
        return new Result(this, Number);
    }

    /// <summary>
    /// Gets the value of the constant.
    /// </summary>
    /// <returns> The value from the expression. </returns>
    public int Value()
    {
        return Result().Value;
    }

    /// <summary>
    /// Generates the string representation of the constant.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString()
    {
        return Number.ToString();
    }
}
