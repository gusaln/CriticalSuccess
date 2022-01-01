namespace CriticalSuccess.Parsing.InputReader;

/// <summary>
/// Provides controlled access into a stream of characters.
/// </summary>
public interface IInputReader
{
    /// <summary>
    /// The next position of the input. The value ranges from -1, when the input is yet to be
    /// read, to N, when the end was reached, where N is the length of the input.
    /// </summary>
    int NextPos { get; }

    /// <summary> Peeks into the next character of the input.</summary>
    /// <remarks> Peeking in SOF state returns <c>null</c>.</remarks>
    /// <returns> The next character or null if the end was reached. </returns>
    char? Peek();

    /// <summary> Peeks into the nth character ahead. </summary>
    /// <remarks> Calling <c>Peek(0)</c> is the same as calling <c>Peek()</c>. </remarks>
    /// <returns> The nth character ahead or null if the end was reached. </returns>
    char? Peek(ushort n);

    /// <summary> Consumes the next character of the input and advances the cursor. </summary>
    /// <remarks> Calling this function in SOF state returns <c>null</c>.</remarks>
    /// <returns> The next character or null if the end was reached. </returns>
    char? Consume();

    /// <summary> Consumes the nth character ahead and advances the cursor to that position. </summary>
    /// <remarks> Calling <c>Consume(0)</c> is the same as calling <c>Consume()</c>. </remarks>
    /// <returns> The nth character ahead or null if the end was reached. </returns>
    char? Consume(ushort n);

    /// <summary> Indicates if the first character of input has not yet been consumed. </summary>
    bool IsSOF();

    /// <summary> Indicates if the last character of input has already been consumed. </summary>
    bool IsEOF();
}
