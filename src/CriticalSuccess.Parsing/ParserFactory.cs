using CriticalSuccess.Parsing.InputReader;
using CriticalSuccess.Parsing.Lexer;

namespace CriticalSuccess.Parsing
{
    /// <summary>
    /// Facilitates the creation of a parser
    /// </summary>
    public class ParserFactory
    {
        /// <summary>
        /// Creates a <c>Parser</c> instance for an input string with a standard <c>ILexer</c> and an <c>InMemoryInputReader</c>.
        /// </summary>
        /// <param name="input"> The input string. </param>
        /// <returns> A <c>Parser</c> instance. </returns>
        public static Parser ForString(string input)
        {
            return new Parser(new Tokenizer(new InMemoryInputReader(input)));
        }
    }
}