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
                config.AddCommand<InteractiveShellCommand>("cli")
                    .WithAlias("shell")
                    .WithDescription("Creates a shell for rolling multiple rolls");
            });

            return app.Run(args);
        }
    }
}
