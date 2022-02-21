

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
        using var updaterClient = new IUpdaterClientConfirmShutdown()
            .OnUpdaterAvailable((machineName) =>
            {
                AnsiConsole.WriteLine($"Updater available on machine {machineName}, {DateTime.Now}");
            })
            .OnUpdateAvailable(async (client) =>
            {
                if (AnsiConsole.Confirm("Start Update?"))
                    await client.UpdateStartAsync();
            })
            .OnConfirmShutdown(async (client) =>
            {
                if (AnsiConsole.Confirm("Shutdown for Update?"))
                    await client.ShutdownAllowedAsync(TimeSpan.FromSeconds(5));
                else
                    await client.ShutdownDeniedAsync();
            })
            .OnInventory(async (client) =>
            {
                await client.InventoryAsync(new List<Updater.CoreLib.grpc.Inventory>
                {
                    new Inventory($"{name}.a.b.c", "Plc", "Plc.1234", "4711.1"),
                    new Inventory($"{name}.e.f.g", "ImageProcessing", "Imag.1234", "4711.2"),
                });
            })
            .Start();


        AnsiConsole.WriteLine("App is Running...");
        Console.ReadLine();

        return 0;
    }
}
