using System;

namespace CriticalSuccess.Parsing.Lexer;

/// <summary>
/// Indicates that an unknown character sequence was reached.
/// </summary>
public class UnknownTokenException : InvalidOperationException
{
    /// <summary> The content of the unknown token. </summary>
    public string Content { get; init; }

    /// <summary> The position of the token. </summary>
    public int Position { get; init; }

    /// <summary>
    /// Creates a new <c>UnknownTokenException</c> instance.
    /// </summary>
    public UnknownTokenException(string content, int position) : base($"Unknown token '{content}' at position {position}.")
    {
        Content = content;
        Position = position;
    }
};
