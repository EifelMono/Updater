namespace Updater.ClientLib;

public interface IUpdaterClientUpdate
{
    Task UpdateStartAsync();
}
public interface IUpdaterClientShutdown
{
    Task ShutdownDeniedAsync();
    Task ShutdownAllowedAsync(TimeSpan timeSpan);
}

public interface IUpdaterClientInventory
{
    Task InventoryAsync(List<Inventory> inventories);
}

public class IUpdaterClientConfirmShutdown : IDisposable,
    IUpdaterClientUpdate, IUpdaterClientShutdown, IUpdaterClientInventory
{
    CancellationTokenSource CancellationTokenSource { get; set; } = null;
    public void Dispose()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource = null;
    }

    public IUpdaterClientConfirmShutdown Start()
    {
        _ = RunAsync();
        return this;
    }

    internal async Task RunAsync()
    {
        CancellationTokenSource = new CancellationTokenSource();
        try
        {
            var udpServer = new UdpServer();
            _ = udpServer.RunAsync(
                (text) =>
                {
                    _onUpdaterAvailable?.Invoke(text);
                },
                CancellationTokenSource);
            await Task.Delay(-1, CancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    #region OnUpdaterAvailable

    protected Action<string> _onUpdaterAvailable;

    public IUpdaterClientConfirmShutdown OnUpdaterAvailable(Action<string> action)
    {
        _onUpdaterAvailable = action;
        return this;
    }

    #endregion

    #region OnUpdateAvailable
    protected Action<IUpdaterClientConfirmShutdown> _onUpdateAvailable;

    public IUpdaterClientConfirmShutdown OnUpdateAvailable(Action<IUpdaterClientUpdate> action)
    {
        _onUpdateAvailable = action;
        return this;
    }

    public Task UpdateStartAsync()
    {
        return Task.CompletedTask;
    }
    #endregion

    #region OnConfirmShutdown
    protected Action<IUpdaterClientConfirmShutdown> _onConfirmShutdown;

    public IUpdaterClientConfirmShutdown OnConfirmShutdown(Action<IUpdaterClientShutdown> action)
    {
        _onConfirmShutdown = action;
        return this;
    }

    public Task ShutdownDeniedAsync()
    {
        return Task.CompletedTask;
    }
    public Task ShutdownAllowedAsync(TimeSpan timeSpan)
    {
        return Task.CompletedTask;
    }
    #endregion

    #region OnInventory
    protected Action<IUpdaterClientInventory> _onInventory;

    public IUpdaterClientConfirmShutdown OnInventory(Action<IUpdaterClientInventory> action)
    {
        _onInventory = action;
        return this;
    }

    public Task InventoryAsync(List<Inventory> inventories)
    {
        return Task.CompletedTask;
    }
    #endregion
}
