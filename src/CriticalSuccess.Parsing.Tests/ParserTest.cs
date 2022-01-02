using System.Collections.Generic;
using System.Linq;
using CriticalSuccess.Parsing.AST;
using CriticalSuccess.Parsing.Lexer;
using Xunit;

namespace CriticalSuccess.Parsing.Tests;

public class ParserTest
{
    [Fact]
    public void Parser_Parse_ThrowsSyntaxErrorExceptionIfSofTokenIsNotTheFirst()
    {
        var parser = new Parser(
            new MockTokenizer(
                new[] { Token.EOFToken(0) }
                )
        );

        var exception = Assert.Throws<SyntaxErrorException>(() =>
        {
            parser.Parse();
        });

        Assert.Equal(Token.EOFToken(0), exception.Token);
        Assert.Equal(
            $"Syntax error at position 0: expected to find the SOF, but found {TokenType.EOF}.",
            exception.Message);
        Assert.Equal(
            $"expected to find the SOF, but found {TokenType.EOF}",
            exception.Reason);
    }

    [Fact]
    public void Parser_Parse_ThrowsSyntaxErrorExceptionIfEofTokenIsNotTheLast()
    {
        var numberToken = createToken("1", TokenType.Number);
        var parser = new Parser(
            new MockTokenizer(new[] {
                Token.SOFToken(),
                numberToken
            })
        );

        var exception = Assert.Throws<SyntaxErrorException>(() =>
        {
            parser.Parse();
        });

        Assert.Equal(numberToken, exception.Token);
        Assert.Equal(
            $"Syntax error at position {numberToken.StartPos}: expected to find the EOF, but found {numberToken.Type}.",
            exception.Message);
        Assert.Equal(
            $"expected to find the EOF, but found {numberToken.Type}",
            exception.Reason);
    }

    [Fact]
    public void Parser_Parse_CanParseAnEmptyProgram()
    {
        var numberToken = createToken("1", TokenType.Number);
        var parser = new Parser(
            new MockTokenizer(new[] {
                Token.SOFToken(),
                Token.EOFToken(0)
            })
        );

        var node = parser.Parse();

        Assert.Equal(new Program(new ExpressionList(new())), node);
    }

    [Fact]
    public void Parser_Parse_CanParsePositiveNumberAsTerminalExpression()
    {
        var numberToken = createToken("1", TokenType.Number);
        var parser = new Parser(
            MockTokenizer.WrappedInFileTerminals(new[] { numberToken })
        );

        var node = parser.Parse();

        Assert.Equal(
            Program.WithExpressions(new[] { (IExpressionNode)new NumberLiteral(numberToken, 1) }),
            node);
    }

    [Fact]
    public void Parser_Parse_CanParseNegativeNumberAsTerminalExpression()
    {
        var numberToken = createToken("-1", TokenType.Number);
        var parser = new Parser(
            MockTokenizer.WrappedInFileTerminals(new[] { numberToken })
        );

        var node = parser.Parse();

        Assert.Equal(
            Program.WithExpressions(new[] { (IExpressionNode)new NumberLiteral(numberToken, -1) }),
            node);
    }

    [Theory]
    [MemberData(nameof(ValidDiceTokenSequencesProvider))]
    public void Parser_Parse_CanParseDiceExpression(IEnumerable<Token> tokens, Program expectedAst)
    {
        var parser = new Parser(MockTokenizer.WrappedInFileTerminals(tokens));
        var node = parser.Parse();

        Assert.Equal(expectedAst, node);
    }

    public static IEnumerable<object[]> ValidDiceTokenSequencesProvider()
    {
        return new object[][] {
            // single die, no modifier
            new object[] {
                new [] {
                    createToken("d", TokenType.DSymbol),
                    createToken("4", TokenType.Number),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode)new DiceRollExpr(
                        Times: null,
                        DSymbol: createToken("d", TokenType.DSymbol),
                        NFaces: createToken("4", TokenType.Number),
                        Modifier: null)
                }),
            },

            // multiple dice, no modifier
            new object[] {
                new [] {
                    createToken("4", TokenType.Number),
                    createToken("d", TokenType.DSymbol),
                    createToken("6", TokenType.Number),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode)new DiceRollExpr(
                        Times: createToken("4", TokenType.Number),
                        DSymbol: createToken("d", TokenType.DSymbol),
                        NFaces: createToken("6", TokenType.Number),
                        Modifier: null)
                }),
            },

            // multiple dice with modifier
            new object[] {
                new [] {
                    createToken("4", TokenType.Number),
                    createToken("d", TokenType.DSymbol),
                    createToken("6", TokenType.Number),
                    createToken("l", TokenType.Modifier),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode)new DiceRollExpr(
                        Times: createToken("4", TokenType.Number),
                        DSymbol: createToken("d", TokenType.DSymbol),
                        NFaces: createToken("6", TokenType.Number),
                        Modifier: createToken("l", TokenType.Modifier))
                }),
            },

            // single die with modifier
            new object[] {
                new [] {
                    createToken("d", TokenType.DSymbol),
                    createToken("6", TokenType.Number),
                    createToken("l", TokenType.Modifier),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode)new DiceRollExpr(
                        Times: null,
                        DSymbol: createToken("d", TokenType.DSymbol),
                        NFaces: createToken("6", TokenType.Number),
                        Modifier: createToken("l", TokenType.Modifier))
                }),
            },
        };
    }

    [Fact]
    public void Parser_Parse_ThrowsSyntaxErrorExceptionIfDSymbolIsNotFollowedByNumber()
    {
        var lexer = MockTokenizer.WrappedInFileTerminals(new[] { createToken("d", TokenType.DSymbol) });
        var parser = new Parser(lexer);

        var exception = Assert.Throws<SyntaxErrorException>(() =>
        {
            parser.Parse();
        });

        var lastToken = lexer.Tokens.Last();
        Assert.Equal(lastToken, exception.Token);
        Assert.Equal(
            $"Syntax error at position {lastToken.EndPos}: number in range [{int.MinValue}, {int.MaxValue}] expected, but found {lastToken.Type}.",
            exception.Message);
        Assert.Equal(
            $"number in range [{int.MinValue}, {int.MaxValue}] expected, but found {lastToken.Type}",
            exception.Reason);
    }

    [Theory]
    [MemberData(nameof(ValidOperationTokenSequencesProvider))]
    public void Parser_Parse_CanParseOperationExpression(IEnumerable<Token> tokens, Program expectedAst)
    {
        var parser = new Parser(MockTokenizer.WrappedInFileTerminals(tokens));
        var node = parser.Parse();

        Assert.Equal(expectedAst, node);
    }

    public static IEnumerable<object[]> ValidOperationTokenSequencesProvider()
    {
        return new object[][] {
            // number-number operation
            new object[] {
                new [] {
                    createToken("1", TokenType.Number),
                    createToken("+", TokenType.Operator),
                    createToken("2", TokenType.Number),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode)new OperationExpr(
                        (IExpressionNode) new NumberLiteral(createToken("1", TokenType.Number), 1),
                        createToken("+", TokenType.Operator),
                        (IExpressionNode) new NumberLiteral(createToken("2", TokenType.Number), 2))
                }),
            },

            // expr-expr-expr operation
            // Given that the parser implements a leftmost derivation algorithm, the resulting expression should be (1+(2+3))
            new object[] {
                new [] {
                    createToken("1", TokenType.Number),
                    createToken("+", TokenType.Operator),
                    createToken("2", TokenType.Number),
                    createToken("+", TokenType.Operator),
                    createToken("3", TokenType.Number),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode)new OperationExpr(
                        (IExpressionNode) new NumberLiteral(createToken("1", TokenType.Number), 1),
                        createToken("+", TokenType.Operator),
                        (IExpressionNode)new OperationExpr(
                            (IExpressionNode) new NumberLiteral(createToken("2", TokenType.Number), 2),
                            createToken("+", TokenType.Operator),
                            (IExpressionNode) new NumberLiteral(createToken("3", TokenType.Number), 3)))
                }),
            },

            // die-number operation
            new object[] {
                new [] {
                    createToken("d", TokenType.DSymbol),
                    createToken("4", TokenType.Number),
                    createToken("+", TokenType.Operator),
                    createToken("1", TokenType.Number),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode) new OperationExpr(
                        (IExpressionNode) new DiceRollExpr(
                            Times: null,
                            DSymbol: createToken("d", TokenType.DSymbol),
                            NFaces: createToken("4", TokenType.Number),
                            Modifier: null),
                        createToken("+", TokenType.Operator),
                        (IExpressionNode) new NumberLiteral(createToken("1", TokenType.Number), 1))
                }),
            },

            // number-die operation
            new object[] {
                new [] {
                    createToken("1", TokenType.Number),
                    createToken("+", TokenType.Operator),
                    createToken("d", TokenType.DSymbol),
                    createToken("4", TokenType.Number),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode) new OperationExpr(
                        (IExpressionNode) new NumberLiteral(createToken("1", TokenType.Number), 1),
                        createToken("+", TokenType.Operator),
                        (IExpressionNode) new DiceRollExpr(
                            Times: null,
                            DSymbol: createToken("d", TokenType.DSymbol),
                            NFaces: createToken("4", TokenType.Number),
                            Modifier: null)
                        )}),
            },

            // die-die operation
            new object[] {
                new [] {
                    createToken("d", TokenType.DSymbol),
                    createToken("6", TokenType.Number),
                    createToken("+", TokenType.Operator),
                    createToken("d", TokenType.DSymbol),
                    createToken("4", TokenType.Number),
                },
                Program.WithExpressions(new[] {
                    (IExpressionNode) new OperationExpr(
                        (IExpressionNode) new DiceRollExpr(
                            Times: null,
                            DSymbol: createToken("d", TokenType.DSymbol),
                            NFaces: createToken("6", TokenType.Number),
                            Modifier: null),
                        createToken("+", TokenType.Operator),
                        (IExpressionNode) new DiceRollExpr(
                            Times: null,
                            DSymbol: createToken("d", TokenType.DSymbol),
                            NFaces: createToken("4", TokenType.Number),
                            Modifier: null)
                        )}),
            },
        };
    }

    [Fact]
    public void Parser_Parse_ThrowsSyntaxErrorExceptionIfOperatorIsNotFollowedByExpr()
    {
        var lexer = MockTokenizer.WrappedInFileTerminals(new[] { createToken("d", TokenType.DSymbol) });
        var parser = new Parser(lexer);

        var exception = Assert.Throws<SyntaxErrorException>(() =>
        {
            parser.Parse();
        });

        var lastToken = lexer.Tokens.Last();
        Assert.Equal(lastToken, exception.Token);
        Assert.Equal(
            $"Syntax error at position {lastToken.EndPos}: number in range [{int.MinValue}, {int.MaxValue}] expected, but found {lastToken.Type}.",
            exception.Message);
        Assert.Equal(
            $"number in range [{int.MinValue}, {int.MaxValue}] expected, but found {lastToken.Type}",
            exception.Reason);
    }

    [Fact]
    public void Parser_Parse_CanParseAnExpressionList()
    {
        var parser = new Parser(MockTokenizer.WrappedInFileTerminals(new[] {
            createToken("1", TokenType.Number),
            createToken(";", TokenType.ExpressionSeparatorSymbol),
            createToken("2", TokenType.Number),
        }));

        var node = parser.Parse();

        Assert.Equal(
            Program.WithExpressions(new[] {
                (IExpressionNode)new NumberLiteral(createToken("1", TokenType.Number), 1),
                (IExpressionNode)new NumberLiteral(createToken("2", TokenType.Number), 2),
            }), node);
    }


    [Fact]
    public void Parser_Parse_ThrowsSyntaxErrorExceptionIfExpressionSeparatorSymbolIsNotFollowedByExpr()
    {
        var lexer = MockTokenizer.WrappedInFileTerminals(new[] {
            createToken("1", TokenType.Number),
            createToken(";", TokenType.ExpressionSeparatorSymbol),
        });
        var parser = new Parser(lexer);

        var exception = Assert.Throws<SyntaxErrorException>(() =>
        {
            parser.Parse();
        });

        var lastToken = lexer.Tokens.Last();
        Assert.Equal(lastToken, exception.Token);
        Assert.Equal(
            $"Syntax error at position {lastToken.EndPos}: number in range [{int.MinValue}, {int.MaxValue}] expected, but found {lastToken.Type}.",
            exception.Message);
        Assert.Equal(
            $"number in range [{int.MinValue}, {int.MaxValue}] expected, but found {lastToken.Type}",
            exception.Reason);
    }

    public static Token createToken(string content, TokenType type)
    {
        return createToken(content, type, 0);
    }

    public static Token createToken(string content, TokenType type, int startPos)
    {
        return new Token { StartPos = startPos, EndPos = content.Length + startPos - 1, Content = content, Type = type };
    }
}

internal class MockTokenizer : ITokenizer
{
    public List<Token> Tokens { get; init; }
    private int _cursor = 0;

    public MockTokenizer(IEnumerable<Token> tokens)
    {
        Tokens = new List<Token>(tokens);
    }

    public static MockTokenizer WrappedInFileTerminals(IEnumerable<Token> tokens)
    {
        var tokenList = new List<Token>();

        tokenList.Add(Token.SOFToken());
        tokenList.AddRange(tokens);
        tokenList.Add(Token.EOFToken(tokens.Sum((t) => t.Content.Length)));

        return new(tokenList);
    }

    public Token Next()
    {
        if (_cursor >= Tokens.Count)
        {
            return Tokens[Tokens.Count - 1];
        }

        return Tokens[_cursor++];
    }

    public Token Next(int n)
    {
        if (_cursor >= Tokens.Count)
        {
            return Tokens[Tokens.Count - 1];
        }

        var pos = _cursor + n;
        if (pos >= Tokens.Count)
        {
            _cursor = pos = Tokens.Count - 1;
        }

        _cursor++;
        return Tokens[pos];
    }

    public Token Peek()
    {
        if (_cursor >= Tokens.Count)
        {
            return Tokens[Tokens.Count - 1];
        }

        return Tokens[_cursor];
    }

    public Token Peek(int n)
    {
        var pos = _cursor + n;
        if (pos >= Tokens.Count)
        {
            pos = Tokens.Count - 1;
        }

        return Tokens[pos];
    }
}
