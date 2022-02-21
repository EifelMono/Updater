namespace Updater.ClientLib;

public class UpdaterClient : IDisposable
{
    CancellationTokenSource CancellationTokenSource { get; set; } = null;
    public void Dispose()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource = null;
    }

    public UpdaterClient Start()
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

    public UpdaterClient OnUpdaterAvailable(Action<string> action)
    {
        _onUpdaterAvailable = action;
        return this;
    }

    #endregion

    #region OnNewUpdateAvailable
    protected Func<bool> _onUpdateAvailable;

    public UpdaterClient OnUpdateAvailable(Func<bool> action)
    {
        _onUpdateAvailable = action;
        return this;
    }
    #endregion

    #region OnConfirmUpdate
    protected Func<bool> _onConfirmUpdate;

    public UpdaterClient OnConfirmUpdate(Func<bool> action)
    {
        _onConfirmUpdate = action;
        return this;
    }
    #endregion
}
