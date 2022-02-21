

var app = new CommandApp<TestAppCommand>();
return app.Run(args);


internal sealed class TestAppCommand : AsyncCommand<TestAppCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Visible Name for the application")]
        [CommandArgument(0, "[Name]")]
        public string? Name { get; init; }
    }

    public async override Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {

        var name = settings.Name;
        if (string.IsNullOrEmpty(name))
            name = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
        Console.Title = name;
        AnsiConsole.Write(new FigletText(name).LeftAligned().Color(Color.Orange1));

        AnsiConsole.WriteLine("\r\nUpdater started");

        using var updaterClient = new UpdaterClient(name)
            .OnUpdaterAvailable((machineName) =>
            {
                AnsiConsole.WriteLine($"Updater available on machine {machineName}, {DateTime.Now}");
            })
            .OnUpdateAvailable(async (client) =>
            {
                if (AnsiConsole.Confirm("Start Update?"))
                    await client.UpdateStartAsync();
            })
            .OnConfirmUpdate(async (client) =>
            {
                await client.UpdateAllowedAsync(AnsiConsole.Confirm("Confirm Update?"), TimeSpan.FromSeconds(5));
            })
            .OnInventory(async (client) =>
            {
                await client.InventoryAsync(new List<Updater.CoreLib.grpc.InventoryPacket>
                {
                    new Updater.CoreLib.grpc.InventoryPacket
                    {
                        Path=$"{name}.a.b.c",
                        Type= "Plc",
                        Version="Plc.1234",
                        Serialnumber= "4711.1"},
                    new Updater.CoreLib.grpc.InventoryPacket
                    {
                        Path=$"{name}.e.f.g",
                        Type= "ImageProcessing",
                        Version="ImageProcessing.1234",
                        Serialnumber= "4711.2"},

                });
            })
            .Start();

        AnsiConsole.WriteLine("App is Running...");

        Console.ReadLine();

        return 0;
    }
}
