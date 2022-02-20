namespace Updater.ServerLib;

public class UpdaterServer : IDisposable
{
    public void Dispose()
    {
    }

    public Task NotifyUpdateAvailableAsync()
    {
        return Task.CompletedTask;
    }
    public Task StartUpdateQueryAsync()
    {
        return Task.CompletedTask;

    }
    public Task StartUpdateAsync()
    {

        return Task.CompletedTask;
    }

}
