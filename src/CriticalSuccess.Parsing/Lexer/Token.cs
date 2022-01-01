namespace CriticalSuccess.Parsing.Lexer;

/// <summary>
/// Represents a unit of parseable input.
/// </summary>
public record Token
{
    /// <summary> The starting position of the token. </summary>
    public int StartPos { get; init; }

    /// <summary> The ending position of the token. </summary>
    public int EndPos { get; init; }

    /// <summary> The content of the token. </summary>
    public string Content { get; init; }

    /// <summary> The type of token. </summary>
    public TokenType Type { get; init; }

    /// <summary>
    /// Creates a new empty SOF Token.
    /// </summary>
    /// <returns> A new SOF Token. </returns>
    public static Token SOFToken() => SOFToken("", -1);

    /// <summary>
    /// Creates a new SOF Token.
    /// </summary>
    /// <param name="content"> The content of the token (whitespaces). </param>
    /// <param name="endPos"> The position of the token. </param>
    /// <returns> A new SOF Token. </returns>
    public static Token SOFToken(string content, int endPos) => new Token
    {
        Content = content,
        StartPos = -1,
        EndPos = endPos,
        Type = TokenType.SOF
    };

    /// <summary>
    /// Creates a new EOF Token.
    /// </summary>
    /// <param name="pos"> The position of the token. </param>
    /// <returns> A new EOF Token. </returns>
    public static Token EOFToken(int pos) => new Token
    {
        Content = "",
        StartPos = pos,
        EndPos = pos,
        Type = TokenType.EOF
    };
}
