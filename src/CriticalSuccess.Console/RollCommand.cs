using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CriticalSuccess.Parsing;
using CriticalSuccess.Parsing.Lexer;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CriticalSuccess.Console
{
    public class RollCommand : Command<RollCommand.RollCommandSettings>
    {
        public class RollCommandSettings : CommandSettings
        {
            [CommandOption("-p|--prefix")]
            [Description("Prefixes each roll with its expression.")]
            public bool Prefix { get; set; }

            [CommandArgument(0, "<rolls>")]
            public string[] Rolls { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] RollCommandSettings settings)
        {
            var input = (new StringBuilder()).AppendJoin(';', settings.Rolls)
                .ToString();

            try
            {
                foreach (var (value, index) in Parser.ParseString(input).Values().Select((value, i) => (value, i)))
                {
                    if (settings.Prefix)
                    {
                        System.Console.Write("{0}: ", settings.Rolls[index].Trim());
                    }
                    System.Console.WriteLine(value);
                }
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

            return 0;
        }

    }

}
