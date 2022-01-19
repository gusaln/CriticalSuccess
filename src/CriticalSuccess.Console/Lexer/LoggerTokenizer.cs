using CriticalSuccess.Parsing.Lexer;

namespace CriticalSuccess.Console.Lexer;

public class LoggerTokenizer : ITokenizer
{
    private ITokenizer _tokenizer { get; init; }
    public LoggerTokenizer(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public Token Next()
    {
        var token = _tokenizer.Next();

        System.Console.WriteLine($"[tokenizer.Next()] {token}");

        return token;
    }

    public Token Next(int n)
    {
        var token = _tokenizer.Next(n);

        System.Console.WriteLine($"[tokenizer.Next({n})] {token}");

        return token;
    }

    public Token Peek()
    {
        var token = _tokenizer.Peek();

        System.Console.WriteLine($"[tokenizer.Peek())] {token}");

        return token;
    }

    public Token Peek(int n)
    {
        var token = _tokenizer.Peek(n);

        System.Console.WriteLine($"[tokenizer.Peek({n})] {token}");

        return token;
    }
}
