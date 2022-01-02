using System.Collections.Generic;
using CriticalSuccess.Core;
using CriticalSuccess.Parsing.AST;
using CriticalSuccess.Parsing.Lexer;

#nullable enable

namespace CriticalSuccess.Parsing;

/// <summary>
/// Parses an input according to the custom roll grammar.
/// </summary>
///
/// <remarks>
/// This is a LL(k) recursive-descent predictive parser.
/// </remarks>
public class Parser
{
    private readonly ITokenizer _lexer;

    /// <summary>
    /// Creates a new parser with the given tokenizer.
    /// </summary>
    /// <param name="lexer"> An <c>ITokenizer</c> instance </param>
    public Parser(ITokenizer lexer)
    {
        _lexer = lexer;
    }

    /// <summary>
    /// Parses the stream of tokens from the tokenizer into an AST
    /// </summary>
    ///
    /// <exception cref="SyntaxErrorException"> When the parser encounters a syntax error. </exception>
    ///
    /// <exception cref="UnknownTokenException"> When the parser reaches an unknown token. </exception>
    ///
    /// <returns> A <c>Program</c> node. </returns>
    public Program Parse()
    {
        consumeSOF();

        var expressions = parseExpressionList();

        consumeEOF();

        return new Program(expressions);
    }

    private void consumeSOF()
    {
        Token token = _lexer.Next();
        if (token.Type != TokenType.SOF)
        {
            throw new SyntaxErrorException($"expected to find the SOF, but found {token.Type}", token);
        }
    }

    private void consumeEOF()
    {
        Token token = _lexer.Next();
        if (token.Type != TokenType.EOF)
        {
            throw new SyntaxErrorException($"expected to find the EOF, but found {token.Type}", token);
        }
    }

    private ExpressionList parseExpressionList()
    {
        var expressions = new List<IExpressionNode>();

        if (!nextTokenIs(TokenType.EOF))
        {
            expressions.Add(parseExpression());
        }

        while (nextTokenIs(TokenType.ExpressionSeparatorSymbol))
        {
            discardNextToken();
            expressions.Add(parseExpression());

            if (nextTokenIsNotAny(new[] { TokenType.ExpressionSeparatorSymbol, TokenType.EOF }))
            {
                Token token = _lexer.Peek();
                throw new SyntaxErrorException(
                    $"expected to find an operator, an expression separator or the EOF, but found {token.Type}",
                    token);
            }
        }

        return new ExpressionList(expressions);
    }

    private IExpressionNode parseExpression()
    {
        if (isThereATokenBetween(1, 4, TokenType.Operator))
        {
            return (IExpressionNode)parseOperationExpr();
        }

        return (IExpressionNode)parseTerminalExpr();
    }

    private OperationExpr parseOperationExpr()
    {
        IExpressionNode lhs = parseTerminalExpr();
        Token op = parseOperator();
        IExpressionNode rhs = parseExpression();

        return new OperationExpr(lhs, op, rhs);
    }

    private Token parseOperator()
    {
        Token token = _lexer.Peek();
        if (token.Type != TokenType.Operator)
        {
            throw new SyntaxErrorException($"operator (either '+' or '-') expected, but found {token.Type}", token);
        }

        return _lexer.Next();
    }

    private IExpressionNode parseTerminalExpr()
    {
        if (isThereATokenBetween(0, 1, TokenType.DSymbol))
        {
            return parseDiceRollExpr();
        }

        (Token Token, int Value) = parseNumber();

        return new NumberLiteral(Token, Value);
    }

    private DiceRollExpr parseDiceRollExpr()
    {

        Token? times = null;
        if (nextTokenIs(TokenType.Number))
        {
            times = parseByteNumber().Token;
        }

        var dSymbolToken = parseDKeyword();
        var facesToken = parseNumber().Token;

        Token? modifier = null;
        if (nextTokenIs(TokenType.Modifier))
        {
            modifier = parseModifier();
        }

        return new DiceRollExpr(times, dSymbolToken, facesToken, modifier);
    }

    private Token parseModifier()
    {
        Token token = _lexer.Next();
        if (token.Type != TokenType.Modifier)
        {
            throw new SyntaxErrorException($"modifier (either one of 'H', 'h', 'L', 'l' or 'e') expected, but found {token.Type}", token);
        }

        return token;
    }

    private (Token Token, byte Value) parseByteNumber()
    {
        var (token, number) = parseNumber();
        if (number < 0 || number > 255)
        {
            throw new SyntaxErrorException($"number between [1, 255] expected, but found {number}", token);
        }

        return (Token: token, Value: (byte)number);
    }

    private (Token Token, int Value) parseNumber()
    {
        Token token = _lexer.Next();
        if (token.Type != TokenType.Number)
        {
            throw new SyntaxErrorException($"number in range [{int.MinValue}, {int.MaxValue}] expected, but found {token.Type}", token);
        }

        return (Token: token, Value: int.Parse(token.Content));
    }

    private Token parseDKeyword()
    {
        Token token = _lexer.Next();
        if (token.Type != TokenType.DSymbol)
        {
            throw new SyntaxErrorException($"'d' letter expected, but found {token.Type}", token);
        }

        return token;
    }

    private void discardNextToken()
    {
        _lexer.Next();
    }

    private bool isThereATokenBetween(byte start, byte inclusiveMax, TokenType t)
    {
        for (byte i = start; i <= inclusiveMax; i++)
        {
            if (_lexer.Peek(i).Type == t)
            {
                return true;
            }
        }

        return false;
    }

    private bool nextTokenIs(TokenType t)
    {
        return _lexer.Peek().Type == t;
    }

    private bool nextTokenIsAny(IEnumerable<TokenType> types)
    {
        foreach (var t in types)
        {
            if (_lexer.Peek().Type == t)
            {
                return true;
            }
        }

        return false;
    }

    private bool nextTokenIsNot(TokenType t)
    {
        return !nextTokenIs(t);
    }

    private bool nextTokenIsNotAny(IEnumerable<TokenType> types)
    {
        return !nextTokenIsAny(types);
    }

}
