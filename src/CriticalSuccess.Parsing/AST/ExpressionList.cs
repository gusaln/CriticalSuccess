using System;
using System.Collections.Generic;
using System.Linq;

namespace CriticalSuccess.Parsing.AST;

/// <summary>
/// Represents a list of expressions.
/// </summary>
public class ExpressionList : INode
{
    /// <summary> A list containing the expression nodes. </summary>
    public List<IExpressionNode> Expressions { get; init; }

    /// <param name="expressions"> A list containing the expression nodes. </param>
    public ExpressionList(List<IExpressionNode> expressions)
    {
        Expressions = expressions;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not ExpressionList)
        {
            return false;
        }

        if (GetHashCode() == obj.GetHashCode())
        {
            return true;
        }

        var other = (ExpressionList)obj;

        return !Expressions.Except(other.Expressions).Any();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Expressions);
    }

    /// <inheritdoc/>
    public static bool operator ==(ExpressionList a, ExpressionList b)
    {
        return a.Equals(b);
    }

    /// <inheritdoc/>
    public static bool operator !=(ExpressionList a, ExpressionList b)
    {
        return !a.Equals(b);
    }
}
