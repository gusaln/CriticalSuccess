using System.Diagnostics.CodeAnalysis;
using System.Text;
using CriticalSuccess.Parsing;
using CriticalSuccess.Parsing.Lexer;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CriticalSuccess.Console
{
    public class InteractiveShellCommand : Command<InteractiveShellCommand.InteractiveShellCommandSettings>
    {
        public class InteractiveShellCommandSettings : CommandSettings
        {
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] InteractiveShellCommandSettings settings)
        {
            writeHelpMessage();
            while (true)
            {
                string input = AnsiConsole.Ask<string>("> ")
                    .Trim()
                    .ToLowerInvariant();

                switch (input)
                {
                    case "c":
                        return 0;
                    case "?":
                        writeHelpMessage();
                        continue;
                    default:
                        break;
                }

                try
                {
                    var rolls = Parser.ParseString(input);
                    var result = rolls.Results();
                    System.Console.WriteLine((new StringBuilder())
                        .AppendJoin(',', rolls.Values())
                        .ToString());
                }
                catch (UnknownTokenException ex)
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(input);
                    System.Console.WriteLine($"{{0, {ex.Position + 1}}}", '^');
                }
                catch (SyntaxErrorException ex)
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(input);
                    System.Console.WriteLine($"{{0, {ex.Token.StartPos + 1}}}", '^');
                }
            }

            return 0;
        }

        private void writeHelpMessage()
        {
            System.Console.WriteLine("Enter an expression or a command and press enter");
            System.Console.WriteLine("Commands: [c] to close the shell, [?] to print this message");
        }
    }

}
