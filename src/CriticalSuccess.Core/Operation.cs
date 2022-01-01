using System;

namespace CriticalSuccess.Core;

/// <summary>
/// Represents an operations between two other expressions.
/// </summary>
public record Operation : IExpression
{
    /// <summary> The operator. </summary>
    public readonly Operator Op;

    /// <summary> The left-hand side of the operation. </summary>
    public readonly IExpression Left;

    /// <summary> The right-hand side of the operation. </summary>
    public readonly IExpression Right;

    /// <summary>
    /// Creates a new operation.
    /// </summary>
    /// <param name="operator">The operator.</param>
    /// <param name="left">The left-hand side IExpression.</param>
    /// <param name="right">The right-hand side IExpression.</param>
    public Operation(Operator @operator, IExpression left, IExpression right)
    {
        Op = @operator;
        Left = left;
        Right = right;
    }

    /// <summary>
    /// Operates on the values of each side and returns the Result.
    /// </summary>
    /// <returns>The <c>Result</c> oof the operation.</returns>
    public Result Result()
    {
        var lhsResult = Left.Result();
        var rhsResult = Right.Result();

        int value = Op switch
        {
            Operator.Add => lhsResult.Value + rhsResult.Value,
            Operator.Sub => lhsResult.Value - rhsResult.Value,
            _ => throw new Exception("Operator not recognized")
        };

        return new Result(this, value, new Result[] { lhsResult, rhsResult });
    }

    /// <summary>
    /// Gets the value of the operation.
    /// </summary>
    /// <remarks> Multiple calls to this method could return different values. </remarks>
    /// <returns> The value from the expression. </returns>
    public int Value()
    {
        return Result().Value;
    }

    /// <summary>
    /// Generates the string representation of the operation.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString()
    {
        char sign = Op switch
        {
            Operator.Add => '+',
            Operator.Sub => '-',
            _ => throw new System.Exception("Operator not recognized")
        };

        return $"{Left.ToString()} {sign} {Right.ToString()}";
    }
}
