using System.Text;
using CriticalSuccess.Parsing;
using CriticalSuccess.Parsing.Lexer;
using Spectre.Console.Cli;

namespace CriticalSuccess.Console
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            System.Console.Title = "CriticalSuccess";

            var app = new CommandApp<RollCommand>();

            app.Configure((config) =>
            {
                config.AddCommand<InteractiveRollCommand>("cli")
                    .WithAlias("roll")
                    .WithDescription("Creates a console for rolling multiple rolls");
            });

            return app.Run(args);
        }
    }
}
