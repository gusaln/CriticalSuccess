namespace CriticalSuccess.Parsing.Lexer;

/// <summary>
/// Represents a recognizable type of token.
/// </summary>
public enum TokenType
{
    /// <summary> The first token emitted. </summary>
    SOF,
    /// <summary> A literal number. </summary>
    Number,
    /// <summary> The die symbol. </summary>
    DSymbol,
    /// <summary> A die roll modifier. </summary>
    Modifier,
    /// <summary> An operator. </summary>
    Operator,
    /// <summary> An expression separator symbol. </summary>
    ExpressionSeparatorSymbol,
    /// <summary> The last token emitted. </summary>
    EOF
}
