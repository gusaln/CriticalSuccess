using System.Linq;
using System.Collections.Generic;

namespace CriticalSuccess.Core;

/// <summary>
/// Stores the value produce by an expression and the steps taken to arrived at that result.
/// </summary>
public class Result
{
    /// <summary>
    /// The expression that produce this result.
    /// </summary>
    public IExpression Expression { get; init; }

    /// <summary>
    /// The value of the result
    /// </summary>
    public int Value { get; init; }

    /// <summary>
    /// The list of the steps that produce the value of the result.
    /// </summary>
    public IReadOnlyCollection<Result> Steps { get; init; }

    /// <summary>
    /// Creates a new instance given an expression and a value.
    /// </summary>
    /// <param name="expression">The expression that produce the result.</param>
    /// <param name="value">The value of the result.</param>
    public Result(IExpression expression, int value)
    {
        Expression = expression;
        Value = value;
        Steps = new Result[] { };
    }

    /// <summary>
    /// Creates a new instance given an expression, a value.
    /// </summary>
    /// <param name="expression">The expression that produce the result.</param>
    /// <param name="value">The value of the result.</param>
    /// <param name="steps">The list of steps that produce the value of the result.</param>
    public Result(IExpression expression, int value, IEnumerable<Result> steps)
    {
        Expression = expression;
        Value = value;
        Steps = steps.ToArray();
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not Result)
        {
            return false;
        }

        if (GetHashCode() == obj.GetHashCode())
        {
            return true;
        }

        var other = (Result)obj;

        return Expression.Equals(other.Expression)
            || Value == other.Value
            || Steps.Count == other.Steps.Count
            || !Steps.Except(other.Steps).Any();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <inheritdoc/>
    public static bool operator ==(Result a, Result b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(Result a, Result b)
    {
        return !a.Equals(b);
    }
}
