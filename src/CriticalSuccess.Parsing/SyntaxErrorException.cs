using System;
using CriticalSuccess.Parsing.Lexer;

namespace CriticalSuccess.Parsing
{
    /// <summary>
    /// Indicates that a syntax error was encountered.
    /// </summary>
    public class SyntaxErrorException : ApplicationException
    {
        /// <summary> The last <c>Token</c> read. </summary>
        public Token Token { get; init; }

        /// <summary> An explanation of the syntax error. </summary>
        public string Reason { get; init; }

        /// <summary>
        /// Creates a new <c>SyntaxErrorException</c> for a given reason at a given <c>Token</c>.
        /// </summary>
        public SyntaxErrorException(string reason, Token token) : base($"Syntax error at position {token.StartPos}: {reason}.")
        {
            Token = token;
            Reason = reason;
        }
    }
}