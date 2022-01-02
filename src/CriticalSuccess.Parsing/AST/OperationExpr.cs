using CriticalSuccess.Parsing.Lexer;

namespace CriticalSuccess.Parsing.AST;

/// <summary>
/// Represents an operation.
/// </summary>
/// <param name="Left"> Left-hand side operand expression. </param>
/// <param name="Operator"> Operator </param>
/// <param name="Right"> Right-hand side operand expression. </param>
/// <returns></returns>
public record OperationExpr(IExpressionNode Left, Token Operator, IExpressionNode Right) : IExpressionNode
{
}
