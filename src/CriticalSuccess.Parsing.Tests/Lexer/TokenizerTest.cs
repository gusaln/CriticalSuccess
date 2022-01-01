using System.Collections.Generic;
using CriticalSuccess.Parsing.InputReader;
using CriticalSuccess.Parsing.Lexer;
using Xunit;

namespace CriticalSuccess.Parsing.Tests.Lexer;

public class TokenizerTest
{
    protected ITokenizer createLexer(string input) => new Tokenizer(new MockInputReader(input));

    protected ITokenizer createLexer(string input, int cursor) => new Tokenizer(new MockInputReader(input, cursor));

    [Theory]
    [MemberData(nameof(TokensDataProvider))]
    public void Tokenizer_ParsesTokensCorrectly(string input, int cursor, Token token)
    {
        Assert.Equal(
            token,
            createLexer(input, cursor).Next()
        );
    }

    public static IEnumerable<object[]> TokensDataProvider()
    {
        return new object[][] {
                // SOF
                new object[] {"", -1, new Token(){StartPos=-1, EndPos=-1, Content="", Type=TokenType.SOF}},
                new object[] {"anything", -1, new Token(){StartPos=-1, EndPos=-1, Content="", Type=TokenType.SOF}},
                new object[] {"   anything", -1, new Token(){StartPos=-1, EndPos=2, Content="   ", Type=TokenType.SOF}},

                // Number
                new object[] {"0", 0, new Token(){StartPos=0, EndPos=0, Content="0", Type=TokenType.Number}},
                new object[] {"1 234", 2, new Token(){StartPos=2, EndPos=4, Content="234", Type=TokenType.Number}},
                new object[] {"123 4", 0, new Token(){StartPos=0, EndPos=3, Content="123 ", Type=TokenType.Number}},
                new object[] {"-234 5", 0, new Token(){StartPos=0, EndPos=4, Content="-234 ", Type=TokenType.Number}},
                new object[] {int.MaxValue.ToString(), 0, new Token(){StartPos=0, EndPos=9, Content=int.MaxValue.ToString(), Type=TokenType.Number}},
                // This number is lexically right (it is a number), but its bigger than the syntax expects.
                new object[] {long.MaxValue.ToString(), 0, new Token(){StartPos=0, EndPos=18, Content=long.MaxValue.ToString(), Type=TokenType.Number}},

                // DSymbol
                new object[] {"d", 0, new Token(){StartPos=0, EndPos=0, Content="d", Type=TokenType.DSymbol}},
                new object[] {"D", 0, new Token(){StartPos=0, EndPos=0, Content="D", Type=TokenType.DSymbol}},
                new object[] {"d d", 0, new Token(){StartPos=0, EndPos=1, Content="d ", Type=TokenType.DSymbol}},
                new object[] {"D D", 0, new Token(){StartPos=0, EndPos=1, Content="D ", Type=TokenType.DSymbol}},
                new object[] {"ddddd", 0, new Token(){StartPos=0, EndPos=0, Content="d", Type=TokenType.DSymbol}},
                new object[] {"DDDDD", 0, new Token(){StartPos=0, EndPos=0, Content="D", Type=TokenType.DSymbol}},

                // Modifier
                new object[] {"h", 0, new Token(){StartPos=0, EndPos=0, Content="h", Type=TokenType.Modifier}},
                new object[] {"h ", 0, new Token(){StartPos=0, EndPos=1, Content="h ", Type=TokenType.Modifier}},
                new object[] {"l", 0, new Token(){StartPos=0, EndPos=0, Content="l", Type=TokenType.Modifier}},
                new object[] {"l ", 0, new Token(){StartPos=0, EndPos=1, Content="l ", Type=TokenType.Modifier}},
                new object[] {"H", 0, new Token(){StartPos=0, EndPos=0, Content="H", Type=TokenType.Modifier}},
                new object[] {"H ", 0, new Token(){StartPos=0, EndPos=1, Content="H ", Type=TokenType.Modifier}},
                new object[] {"L", 0, new Token(){StartPos=0, EndPos=0, Content="L", Type=TokenType.Modifier}},
                new object[] {"L ", 0, new Token(){StartPos=0, EndPos=1, Content="L ", Type=TokenType.Modifier}},
                new object[] {"e", 0, new Token(){StartPos=0, EndPos=0, Content="e", Type=TokenType.Modifier}},
                new object[] {"e ", 0, new Token(){StartPos=0, EndPos=1, Content="e ", Type=TokenType.Modifier}},

                // Operator
                new object[] {"+", 0, new Token(){StartPos=0, EndPos=0, Content="+", Type=TokenType.Operator}},
                new object[] {"+ ", 0, new Token(){StartPos=0, EndPos=1, Content="+ ", Type=TokenType.Operator}},
                new object[] {"-", 0, new Token(){StartPos=0, EndPos=0, Content="-", Type=TokenType.Operator}},
                new object[] {"- ", 0, new Token(){StartPos=0, EndPos=1, Content="- ", Type=TokenType.Operator}},

                // ExpressionSeparatorSymbol
                new object[] {";", 0, new Token(){StartPos=0, EndPos=0, Content=";", Type=TokenType.ExpressionSeparatorSymbol}},
                new object[] {"; ", 0, new Token(){StartPos=0, EndPos=1, Content="; ", Type=TokenType.ExpressionSeparatorSymbol}},

                // EOF
                new object[] {"foo", 3, new Token(){StartPos=3, EndPos=3, Content="", Type=TokenType.EOF}},
            };
    }

    [Fact]
    public void Tokenizer_KeepsOutputtingEOFAfterEOF()
    {
        var lexer = createLexer("", 0);

        Assert.Equal(TokenType.EOF, lexer.Next().Type);
        Assert.Equal(TokenType.EOF, lexer.Next().Type);
    }

    [Fact]
    public void Tokenizer_OutputsSOFFirst()
    {
        var lexer = createLexer("");

        Assert.Equal(TokenType.SOF, lexer.Next().Type);
        Assert.NotEqual(TokenType.SOF, lexer.Next().Type);
    }

    [Fact]
    public void Tokenizer_PeeksWithoutConsumingTokens()
    {
        var lexer = createLexer("");

        Assert.Equal(lexer.Peek(), lexer.Peek());
    }

    [Fact]
    public void Tokenizer_PeeksWithoutMovingTheCursor()
    {
        var reader = new MockInputReader(" 1");
        var lexer = new Tokenizer(reader);

        // For the tokenizer to read a token...
        lexer.Peek();
        var pos = reader.NextPos;

        // and after peeking again;
        lexer.Peek();

        Assert.Equal(pos, reader.NextPos);
    }

    [Fact]
    public void Tokenizer_CanPeekAhead()
    {
        var lexer = createLexer("1 2 3 4 5", 0);

        Assert.Equal("1 ", lexer.Peek().Content);
        Assert.Equal("2 ", lexer.Peek(1).Content);
        Assert.Equal("5", lexer.Peek(4).Content);
    }

    [Fact]
    public void Tokenizer_CanConsumeTokens()
    {
        var lexer = createLexer("1 2 3 4 5", 0);

        Assert.Equal("1 ", lexer.Next().Content);
        Assert.Equal("2 ", lexer.Next().Content);
        Assert.Equal("3 ", lexer.Next().Content);
    }

    [Fact]
    public void Tokenizer_Next_CanConsumeTokensAhead()
    {
        var lexer = createLexer("1 2 3 4 5", 0);

        Assert.Equal("1 ", lexer.Next().Content);
        Assert.Equal("3 ", lexer.Next(1).Content);
        Assert.Equal("5", lexer.Next(1).Content);
    }

    [Fact]
    public void Tokenizer_Next_ThrowsUnknownTokenException()
    {
        var exception = Assert.Throws<UnknownTokenException>(() =>
        {
            // The spaces in the input string are purposefully placed.
            // The exception should only contain the next character without the extra spaces.
            createLexer("1 x ", 2).Next();
        });

        Assert.Equal("Unknown token 'x' at position 2.", exception.Message);
        Assert.Equal("x", exception.Content);
        Assert.Equal(2, exception.Position);
    }


    [Fact]
    public void Tokenizer_Peek_ThrowsUnknownTokenException()
    {
        var exception = Assert.Throws<UnknownTokenException>(() =>
        {
            // The spaces in the input string are purposefully placed.
            // The exception should only contain the next character without the extra spaces.
            createLexer("1 x ", 2).Peek();
        });

        Assert.Equal("Unknown token 'x' at position 2.", exception.Message);
        Assert.Equal("x", exception.Content);
        Assert.Equal(2, exception.Position);
    }
}

/// <summary>
/// Mocks the input reader.
/// </summary>
///
/// <remarks> This class copies a tested version of an in memory input. </remarks>
internal class MockInputReader : IInputReader
{
    public int NextPos { get => _cursor; }

    private readonly string _input;

    private int _cursor = -1;

    public MockInputReader(string input) => _input = input;

    public MockInputReader(string input, int cursor)
    {
        _input = input;
        _cursor = cursor;
    }

    public char? Peek()
    {
        if (IsSOF())
        {
            return null;
        }

        return IsEOF() ? null : _input[_cursor];
    }

    public char? Consume()
    {
        if (IsSOF())
        {
            _cursor++;

            return null;
        }

        return IsEOF() ? null : _input[_cursor++];
    }

    public char? Peek(ushort n)
    {
        int i = n + _cursor;
        if (i >= (_input.Length))
        {
            return null;
        }

        return _input[i];
    }

    public char? Consume(ushort n)
    {
        int i = n + _cursor;
        if (i >= _input.Length)
        {
            _cursor = _input.Length;
            return null;
        }

        _cursor = i + 1;

        return _input[i];
    }

    public bool IsSOF() => _cursor < 0;

    public bool IsEOF() => _cursor >= _input.Length;
}
