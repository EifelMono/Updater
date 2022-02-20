

namespace Updater.ClientLib;

public class UpdaterClient : IDisposable
{
    public void Dispose()
    {
        Stop();
    }

    public UpdaterClient Start()
    {
        return this;
    }


    public void Stop()
    {

    }

    protected Func<bool> _onNewUpdateAvailable;

    public UpdaterClient OnNewUpdateAvailable(Func<bool> action)
    {
        _onNewUpdateAvailable = action;
        return this;
    }

    protected void StartUpdate()
    {

    }

    protected Func<bool> _onStartUpdateQuery;


    public UpdaterClient OnStartUpdateQuery(Func<bool> action)
    {
        _onStartUpdateQuery = action;
        return this;
    }

    protected void StartUpdateQueryResponse()
    {

    }
}
