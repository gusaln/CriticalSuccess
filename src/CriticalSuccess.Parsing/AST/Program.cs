using System.Collections.Generic;
using System.Linq;

namespace CriticalSuccess.Parsing.AST;

/// <summary>
/// Represents the base node of every dice roll generated.
/// </summary>
/// <param name="ExpressionList"> A node containing a list of expressions. </param>
public record Program(ExpressionList ExpressionList) : INode
{
    /// <summary>
    /// Creates a <c>Program</c> node that contains an <c>ExpressionList</c> instance with the expressions given.
    /// </summary>
    /// <param name="nodes"> An <c>IEnumerable</c> containing expression nodes. </param>
    public static Program WithExpressions(IEnumerable<IExpressionNode> nodes)
    {
        return new Program(new ExpressionList(nodes.ToList()));
    }
}
