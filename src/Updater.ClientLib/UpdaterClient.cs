using Grpc.Core;
using Updater.CoreLib;

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

public class UpdaterClient : IDisposable,
    IUpdaterClientUpdate, IUpdaterClientShutdown, IUpdaterClientInventory
{

    private string Name { get; set; }
    public UpdaterClient(string name)
    {
        Name = name;
    }

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

    internal Task RunAsync()
    {
        return Task.Run(async () =>
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
        });
    }

    #region OnUpdaterAvailable

    protected Action<string> _onUpdaterAvailable;

    public UpdaterClient OnUpdaterAvailable(Action<string> action)
    {
        _onUpdaterAvailable = action;
        return this;
    }

    #endregion

    #region OnUpdateAvailable
    protected Action<UpdaterClient> _onUpdateAvailable;

    public UpdaterClient OnUpdateAvailable(Action<IUpdaterClientUpdate> action)
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
    protected Action<UpdaterClient> _onConfirmShutdown;

    public UpdaterClient OnConfirmShutdown(Action<IUpdaterClientShutdown> action)
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

    public UpdaterClient OnInventory(Action<IUpdaterClientInventory> action)
    {
        _onInventory = action;
        return this;
    }

    public Task InventoryAsync(List<Inventory> inventories)
    {
        return Task.CompletedTask;
    }
    #endregion


    public Task SayHelloAsync()
    {
        Channel channel = new Channel($"127.0.0.1:{Globals.grpcPort}", ChannelCredentials.Insecure);

        var client = new Greeter.GreeterClient(channel);
        var reply = client.SayHello(new HelloRequest { Name = Name });
        Console.WriteLine(reply);
        return Task.CompletedTask;
    }
}
