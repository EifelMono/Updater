var name = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
AnsiConsole.Write(new FigletText(name).LeftAligned().Color(Color.Cyan1));


using var updaterServer = new UpdaterServer();

await updaterServer.BroadcastUpdaterAvailableAsync();

var exitCommand = "Exit";

var selections = new Dictionary<string, Func<Task>>
{
    ["Grpc reconnect"] = async () => await updaterServer.BroadcastGrpcReconnectAsync(),
    ["Updater available"] = async () => await updaterServer.BroadcastUpdaterAvailableAsync(),
    ["Inventory"] = async () => await updaterServer.BroadcastInventoryAsync(TimeSpan.FromSeconds(2)),
    ["Update available"] = async () => await updaterServer.BroadcastUpdateAvailableAsync(),
    ["Confirme Update "] = async () => await updaterServer.BroadcastConfirmUpdateAsync(),
    ["Start Update"] = async () => await updaterServer.BroadcastStartUpdateAsync(),
    [exitCommand] = () => Task.CompletedTask
};

var run = true;
while (run)
{
    var select = AnsiConsole.Prompt(
                  new SelectionPrompt<string>()
                      .Title($"\r\n[cyan1]Select you action[/]")
                      .HighlightStyle(new Style(Color.Cyan1))
                      .PageSize(12)
                      .AddChoices(selections.Keys));
    run = select != exitCommand;
    await selections[select].Invoke();
}

return 0;
