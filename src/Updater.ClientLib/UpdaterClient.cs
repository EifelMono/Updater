using Grpc.Core;
using Updater.CoreLib;

namespace Updater.ClientLib;

public interface IUpdaterClientUpdate
{
    Task UpdateStartAsync();
}
public interface IUpdaterClientConfirmUpdate
{
    Task UpdateAllowedAsync(bool allowed, TimeSpan timeSpan);
}

public interface IUpdaterClientInventory
{
    Task InventoryAsync(List<Updater.CoreLib.grpc.InventoryPacket> inventories);
}

public class UpdaterClient : IDisposable,
    IUpdaterClientUpdate, IUpdaterClientConfirmUpdate, IUpdaterClientInventory
{

    private string Name { get; set; }
    public UpdaterClient(string name)
    {
        Name = name;
    }

    CoreLib.grpc.Updater.UpdaterClient GrpcUpdaterClient { get; set; } = null;
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
                    (message) =>
                    {
                        var broadcastMessage = message.FromJson<BroadcastMessage>();
                        if (broadcastMessage is null)
                            return;
                        if (broadcastMessage.MachineName != Environment.MachineName)
                            return;
                        if (GrpcUpdaterClient is null || broadcastMessage.Command == Command.GrpcReconnect)
                        {
                            Channel channel = new Channel($"127.0.0.1:{Globals.grpcPort}", ChannelCredentials.Insecure);
                            GrpcUpdaterClient = new CoreLib.grpc.Updater.UpdaterClient(channel);
                        }
                        switch (broadcastMessage.Command)
                        {

                            case Command.UpdaterAvailable:
                                _onUpdaterAvailable?.Invoke(broadcastMessage);
                                break;
                            case Command.Inventory:
                                _onInventory?.Invoke(this);
                                break;
                            case Command.UpdateAvailable:
                                _onUpdateAvailable?.Invoke(this);
                                break;
                            case Command.ConfirmUpdate:
                                _onConfirmUpdate.Invoke(this);
                                break;

                        }
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

    protected Action<BroadcastMessage> _onUpdaterAvailable;

    public UpdaterClient OnUpdaterAvailable(Action<BroadcastMessage> action)
    {
        _onUpdaterAvailable = action;
        return this;
    }

    #endregion

    #region OnUpdateAvailable
    protected Action<IUpdaterClientUpdate> _onUpdateAvailable;

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
    protected Action<IUpdaterClientConfirmUpdate> _onConfirmUpdate;

    public UpdaterClient OnConfirmUpdate(Action<IUpdaterClientConfirmUpdate> action)
    {
        _onConfirmUpdate = action;
        return this;
    }

    public Task UpdateAllowedAsync(bool allowed, TimeSpan timeSpan)
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

    public async Task InventoryAsync(List<Updater.CoreLib.grpc.InventoryPacket> inventories)
    {
        if (GrpcUpdaterClient is { })
        {
            var request = new InventoryRequest();
            request.Packet.AddRange(inventories);
            _ = await GrpcUpdaterClient.SendInventoryAsync(request);
        }
    }
    #endregion
}
