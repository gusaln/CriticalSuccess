using CriticalSuccess.Parsing.Lexer;

namespace CriticalSuccess.Parsing.AST;

/// <summary>
/// Represents a literal number.
/// </summary>
/// <param name="Token"> Token containing the number. </param>
/// <param name="Value"> The value of the number. </param>
public record NumberLiteral(Token Token, int Value) : IExpressionNode
{
}
