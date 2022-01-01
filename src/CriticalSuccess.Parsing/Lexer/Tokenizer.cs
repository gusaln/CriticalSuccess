using System.Collections.Generic;
using System.Text;
using CriticalSuccess.Core.Serialization;
using CriticalSuccess.Parsing.InputReader;

namespace CriticalSuccess.Parsing.Lexer;

delegate bool Condition(char c);

/// <summary>
/// Transforms a stream of characters into a stream of <c>Token</c>s.
/// </summary>
/// <remarks> The Tokenizer concatenates whitespaces at the end of the Tokens. </remarks>
public class Tokenizer : ITokenizer
{
    private readonly IInputReader _inputReader;

    private readonly Queue<Token> _buffer;

    /// <summary>
    /// Creates a <c>Tokenizer</c> instance that will read character from an <c>IInputReader</c>.
    ///
    /// </summary>
    /// <param name="inputReader"> An <c>IInputReader</c> instance </param>
    public Tokenizer(IInputReader inputReader)
    {
        _inputReader = inputReader;
        _buffer = new Queue<Token>(8);
    }

    /// <summary>
    /// Peeks the next token in the stream witout advancing the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    public Token Peek()
    {
        if (_buffer.Count > 0)
        {
            return _buffer.Peek();
        }

        Token nextToken = consume();

        _buffer.Enqueue(nextToken);

        return nextToken;
    }

    /// <summary>
    /// Peeks the nth token ahead in the stream witout advancing the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    public Token Peek(int n)
    {
        for (int i = 0; _buffer.Count <= n; i++)
        {
            _buffer.Enqueue(consume());

            if (_inputReader.IsEOF())
            {
                return _buffer.ToArray()[_buffer.Count - 1];
            }
        }

        return _buffer.ToArray()[n];
    }

    /// <summary>
    /// Consumes the next token in the stream and advances the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    public Token Next()
    {
        if (_buffer.Count > 0)
        {
            return _buffer.Dequeue();
        }

        return consume();
    }

    /// <summary>
    /// Consumes up to and including nth token ahead in the stream and advances the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    public Token Next(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Next();
        }

        return Next();
    }

    private Token consume()
    {
        if (_inputReader.IsSOF())
        {
            return processSOF();
        }

        if (_inputReader.IsEOF())
        {
            return processEOF();
        }

        char nextChar = (char)_inputReader.Peek();
        if (char.IsDigit(nextChar) || (nextChar == '-' && char.IsDigit(_inputReader.Peek(1) ?? ' ')))
        {
            return processNumber();
        }
        if (isOperator(nextChar))
        {
            return processOperator();
        }
        if (isDSymbol(nextChar))
        {
            return processDSymbol();
        }
        if (isModifier(nextChar))
        {
            return processModifier();
        }
        if (isExpressionSeparator(nextChar))
        {
            return processExpressionSeparator();
        }

        throw new UnknownTokenException(concatenateWithWhitespaces(nextChar).ToString(), _inputReader.NextPos);
    }

    private Token processSOF()
    {
        _inputReader.Consume();

        return Token.SOFToken(
            content: concatenateWithWhitespaces(new StringBuilder()).ToString(),
            endPos: _inputReader.NextPos >= 0 ? _inputReader.NextPos - 1 : -1);
    }

    private Token processEOF() => Token.EOFToken(_inputReader.NextPos);

    private Token processDSymbol()
    {
        int startPos = _inputReader.NextPos;
        char nextChar = (char)_inputReader.Consume();

        return new Token
        {
            Content = concatenateWithWhitespaces(nextChar).ToString(),
            StartPos = startPos,
            EndPos = _inputReader.NextPos - 1,
            Type = TokenType.DSymbol
        };
    }

    private Token processModifier()
    {
        int startPos = _inputReader.NextPos;
        char nextChar = (char)_inputReader.Consume();

        return new Token
        {
            Content = concatenateWithWhitespaces(nextChar).ToString(),
            StartPos = startPos,
            EndPos = _inputReader.NextPos - 1,
            Type = TokenType.Modifier
        };
    }

    private Token processOperator()
    {
        int startPos = _inputReader.NextPos;
        char nextChar = (char)_inputReader.Consume();

        return new Token
        {
            Content = concatenateWithWhitespaces(nextChar).ToString(),
            StartPos = startPos,
            EndPos = _inputReader.NextPos - 1,
            Type = TokenType.Operator
        };
    }

    private Token processNumber()
    {
        int startPos = _inputReader.NextPos;
        char nextChar = (char)_inputReader.Consume();

        var strBuilder = concatenateWithNext(nextChar, char.IsDigit);
        return new Token
        {
            Content = concatenateWithWhitespaces(strBuilder).ToString(),
            StartPos = startPos,
            EndPos = _inputReader.NextPos - 1,
            Type = TokenType.Number
        };
    }

    private Token processExpressionSeparator()
    {
        int startPos = _inputReader.NextPos;
        char nextChar = (char)_inputReader.Consume();

        return new Token
        {
            Content = concatenateWithWhitespaces(nextChar).ToString(),
            StartPos = startPos,
            EndPos = _inputReader.NextPos - 1,
            Type = TokenType.ExpressionSeparatorSymbol
        };
    }

    private bool isOperator(char c)
    {
        return OperatorMapper.FromChar.ContainsKey(c);
    }

    private bool isModifier(char c)
    {
        return ModifierMapper.FromChar.ContainsKey(c);
    }

    private bool isExpressionSeparator(char c)
    {
        return c == ';';
    }

    private bool isDSymbol(char c)
    {
        return c == 'd' || c == 'D';
    }

    private StringBuilder concatenateWithWhitespaces(char firstChar)
        => concatenateWithNext(firstChar, (c) => c == ' ');

    private StringBuilder concatenateWithWhitespaces(StringBuilder tokenContentBuilder)
        => consumeAndAppend(tokenContentBuilder, (c) => c == ' ');

    private StringBuilder concatenateWithNext(char firstChar, Condition condition)
        => consumeAndAppend(new StringBuilder().Append(firstChar), condition);

    private StringBuilder consumeAndAppend(StringBuilder tokenContentBuilder, Condition condition)
    {
        if (_inputReader.IsEOF())
        {
            return tokenContentBuilder;
        }

        char nextChar = (char)_inputReader.Peek();
        while (condition(nextChar))
        {
            tokenContentBuilder.Append(_inputReader.Consume());

            if (_inputReader.IsEOF())
            {
                break;
            }
            nextChar = (char)_inputReader.Peek();
        }

        return tokenContentBuilder;
    }
}
