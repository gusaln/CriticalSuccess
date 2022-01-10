using System.Collections.Generic;
using System.Linq;
using CriticalSuccess.Core;
using CriticalSuccess.Core.Serialization;
using CriticalSuccess.Parsing.AST;
using CriticalSuccess.Parsing.Lexer;

namespace CriticalSuccess.Parsing;

/// <summary>
/// Generates <c>Rolls</c> from an AST.
/// </summary>
public static class RollsGenerator
{
    /// <summary>
    /// Generates <c>Rolls</c> from an AST.
    /// </summary>
    /// <param name="node"></param>
    public static Rolls Generate(Program node)
    {
        IEnumerable<IExpression> expressions = node.ExpressionList.Expressions.Select(x => map(x));

        return new Rolls(expressions);
    }

    private static IExpression map(IExpressionNode node)
    {
        return (node) switch
        {
            NumberLiteral n => mapConstant(n),
            OperationExpr n => mapOperation(n),
            DiceRollExpr n => mapDice(n),
            _ => throw new System.Exception($"Unknown expression {node.GetType()}."),
        };
    }

    private static Dice mapDice(DiceRollExpr node)
    {
        return (node) switch
        {
            { Times: null, Modifier: null } => new Dice(int.Parse(node.NFaces.Content)),
            { Modifier: null } => new Dice(byte.Parse(node.Times.Content), int.Parse(node.NFaces.Content)),
            { Times: null } => new Dice(1, int.Parse(node.NFaces.Content), mapModifier(node.Modifier)),
            _ => new Dice(byte.Parse(node.Times.Content), int.Parse(node.NFaces.Content), mapModifier(node.Modifier)),
        };
    }

    private static Modifier mapModifier(Token mod)
    {
        if (!ModifierMapper.FromChar.TryGetValue(mod.Content[0], out var modifier))
        {
            throw new System.Exception($"Unknown modifier {mod.Content}. Expected one of 'H', 'h', 'L', 'l' or 'e'.");
        }

        return modifier;
    }

    private static Constant mapConstant(NumberLiteral node)
    {
        return new Constant(node.Value);
    }

    private static Operation mapOperation(OperationExpr node)
    {
        IExpression lhs = node switch
        {
            { Left: NumberLiteral left } => mapConstant(left),
            { Left: DiceRollExpr left } => mapDice(left),
            _ => throw new System.Exception($"Unknown expression {node.Left.GetType()}."),
        };

        IExpression rhs = node switch
        {
            { Right: NumberLiteral right } => mapConstant(right),
            { Right: DiceRollExpr right } => mapDice(right),
            _ => throw new System.Exception($"Unknown expression {node.Right.GetType()}."),
        };

        return new Operation(mapOperator(node.Operator), lhs, rhs);
    }

    private static Operator mapOperator(Token op)
    {
        if (!OperatorMapper.FromChar.TryGetValue(op.Content[0], out var @operator))
        {
            throw new System.Exception($"Unknown operator {op.Content}. Expected either '+' or '-'.");
        }

        return @operator;
    }
}
