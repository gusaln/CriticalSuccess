namespace CriticalSuccess.Parsing.Lexer;

/// <summary>
/// Creates a stream of tokens from an input.
/// </summary>
public interface ITokenizer
{
    /// <summary>
    /// Peeks the next token in the stream witout advancing the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    Token Peek();

    /// <summary>
    /// Peeks the nth token ahead in the stream witout advancing the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    Token Peek(int n);

    /// <summary>
    /// Consumes the next token in the stream and advances the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    Token Next();

    /// <summary>
    /// Consumes up to and including nth token ahead in the stream and advances the cursor.
    /// </summary>
    /// <returns> A <c>Token</c> instance. </returns>
    Token Next(int n);
}
