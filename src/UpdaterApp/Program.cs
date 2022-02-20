var name = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
AnsiConsole.Write(new FigletText(name).LeftAligned().Color(Color.Cyan1));


using var updaterServer = new UpdaterServer();


var exitCommand = "Exit";

var selections = new Dictionary<string, Func<Task>>
{
    ["Notify new update available"] = async () => await updaterServer.NotifyUpdateAvailableAsync(),
    ["Start Update Query"] = async () => await updaterServer.StartUpdateQueryAsync(),
    ["Start Update"] = async () => await updaterServer.StartUpdateAsync(),
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
