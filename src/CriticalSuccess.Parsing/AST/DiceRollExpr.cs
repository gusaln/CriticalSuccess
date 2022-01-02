using CriticalSuccess.Parsing.Lexer;

#nullable enable

namespace CriticalSuccess.Parsing.AST;

/// <summary>
/// Represents a <c>Dice</c>.
/// </summary>
///
/// <param name="Times"> Optional token containing the number of dice. </param>
/// <param name="DSymbol"> Token containing the D symbol. </param>
/// <param name="NFaces"> Token containing the number of faces in the dice. </param>
/// <param name="Modifier"> Optional token containing the modifier for the dice roll. </param>
public record DiceRollExpr(Token? Times, Token DSymbol, Token NFaces, Token? Modifier) : IExpressionNode
{
}
