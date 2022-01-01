namespace CriticalSuccess.Parsing.InputReader;

/// <summary>
/// Provides controlled access to the characters of a string.
/// </summary>
public class InMemoryInputReader : IInputReader
{
    /// <summary>
    /// The next position of the input. The value ranges from -1, when the input is yet to be
    /// read, to N, when the end was reached, where N is the length of the input.
    /// </summary>
    public int NextPos { get => _cursor; }

    private readonly string _input;

    private int _cursor = -1;

    /// <summary>
    /// Creates a new reader for a string.
    /// </summary>
    /// <param name="input"> The input string. </param>
    public InMemoryInputReader(string input) => _input = input;

    /// <summary> Peeks into the next character of the input.</summary>
    /// <remarks> Peeking in SOF state returns <c>null</c>.</remarks>
    /// <returns> The next character or null if the end was reached. </returns>
    public char? Peek()
    {
        if (IsSOF())
        {
            return null;
        }

        return IsEOF() ? null : _input[_cursor];
    }

    /// <summary> Consumes the next character of the input and advances the cursor. </summary>
    /// <remarks> Calling this function in SOF state returns <c>null</c>.</remarks>
    /// <returns> The next character or null if the end was reached. </returns>
    public char? Consume()
    {
        if (IsSOF())
        {
            _cursor++;

            return null;
        }

        return IsEOF() ? null : _input[_cursor++];
    }

    /// <summary> Peeks into the nth character ahead. </summary>
    /// <remarks> Calling <c>Peek(0)</c> is the same as calling <c>Peek()</c>. </remarks>
    /// <returns> The nth character ahead or null if the end was reached. </returns>
    public char? Peek(ushort n)
    {
        int i = n + _cursor;
        if (i >= (_input.Length))
        {
            return null;
        }

        return _input[i];
    }

    /// <summary> Consumes the nth character ahead and advances the cursor to that position. </summary>
    /// <remarks> Calling <c>Consume(0)</c> is the same as calling <c>Consume()</c>. </remarks>
    /// <returns> The nth character ahead or null if the end was reached. </returns>
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

    /// <summary> Indicates if the first character of input has not yet been consumed. </summary>
    public bool IsSOF() => _cursor < 0;

    /// <summary> Indicates if the last character of input has already been consumed. </summary>
    public bool IsEOF() => _cursor >= _input.Length;
}
