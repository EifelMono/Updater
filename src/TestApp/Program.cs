var app = new CommandApp<TestAppCommand>();
return app.Run(args);


internal sealed class TestAppCommand : Command<TestAppCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Visible Name for the application")]
        [CommandArgument(0, "[Name]")]
        public string? Name { get; init; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {

        var name = settings.Name;
        if (string.IsNullOrEmpty(name))
            name = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
        Console.Title = name;
        AnsiConsole.Write(new FigletText(name).LeftAligned().Color(Color.Orange1));


        AnsiConsole.WriteLine("\r\nUpdater started");
        using var updaterClient = new UpdaterClient()
            .OnNewUpdateAvailable(() =>
            {
                return AnsiConsole.Confirm("Please confirm this Update.");
            })
            .OnStartUpdateQuery(() =>
            {
                return AnsiConsole.Confirm("Please confirm the current update now.");
            })
            .Start();


        AnsiConsole.WriteLine("App is Running...");
        Console.ReadLine();

        return 0;
    }
}
