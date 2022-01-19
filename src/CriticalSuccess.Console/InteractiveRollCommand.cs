using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CriticalSuccess.Parsing;
using CriticalSuccess.Parsing.Lexer;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CriticalSuccess.Console
{
    public class InteractiveRollCommand : Command<InteractiveRollCommand.InteractiveRollCommandSettings>
    {
        public class InteractiveRollCommandSettings : CommandSettings
        {
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] InteractiveRollCommandSettings settings)
        {
            while (true)
            {
                System.Console.WriteLine("Commands: [c] to close");
                string input = AnsiConsole.Ask<string>("Input a roll: ")
                    .Trim()
                    .ToLowerInvariant();

                if (input == "c")
                {
                    return 0;
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

    }

}
