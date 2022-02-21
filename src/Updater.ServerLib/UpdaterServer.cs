using Grpc.Core;
using Updater.CoreLib;
using Updater.CoreLib.grpc;
using Updater.CoreLib.udp;

namespace Updater.ServerLib;

class UpdaterImpl : CoreLib.grpc.Updater.UpdaterBase
{
    private UpdaterServer UpdaterServer { get; set; }
    public UpdaterImpl(UpdaterServer updaterServer)
    {
        UpdaterServer = updaterServer;
    }

    public override Task<InventoryReply> SendInventory(InventoryRequest request, ServerCallContext context)
    {
        Console.WriteLine();
        foreach (var inventory in request.Packet)
            Console.WriteLine($"{inventory.Path} {inventory.Type} {inventory.Version} {inventory.Serialnumber}");
        return Task.FromResult(new InventoryReply { });
    }
}

public class UpdaterServer : IDisposable
{
    public CancellationTokenSource CancellationTokenSource { get; set; }
    public Grpc.Core.Server GrpcServer { get; set; }
    public UpdaterServer()
    {
        CancellationTokenSource = new CancellationTokenSource();
        GrpcServer = new Grpc.Core.Server()
        {
            Services = { CoreLib.grpc.Updater.BindService(new UpdaterImpl(this)) },
            Ports = { new ServerPort("0.0.0.0", Globals.grpcPort, ServerCredentials.Insecure) }
        };
        GrpcServer.Start();
    }
    public async void Dispose()
    {
        CancellationTokenSource?.Cancel();
        await GrpcServer?.ShutdownAsync();
    }

    public async Task BroadcastGrpcReconnectAsync()
    {
        await new CoreLib.udp.UdpClient().SendBroadcastMessageAsync(
            new BroadcastMessage { MachineName = Environment.MachineName, Command = Command.GrpcReconnect });
    }

    public async Task BroadcastUpdaterAvailableAsync()
    {
        await new CoreLib.udp.UdpClient().SendBroadcastMessageAsync(
            new BroadcastMessage { MachineName = Environment.MachineName, Command = Command.UpdaterAvailable });
    }
    public async Task BroadcastInventoryAsync(TimeSpan timeSpan)
    {
        await new CoreLib.udp.UdpClient().SendBroadcastMessageAsync(
            new BroadcastMessage { MachineName = Environment.MachineName, Command = Command.Inventory });
        await Task.Delay(timeSpan);
    }


    public async Task BroadcastUpdateAvailableAsync()
    {
        await new CoreLib.udp.UdpClient().SendBroadcastMessageAsync(
            new BroadcastMessage { MachineName = Environment.MachineName, Command = Command.UpdateAvailable });
    }
    public async Task BroadcastConfirmUpdateAsync()
    {
        await new CoreLib.udp.UdpClient().SendBroadcastMessageAsync(
            new BroadcastMessage { MachineName = Environment.MachineName, Command = Command.ConfirmUpdate });
    }

    public async Task BroadcastStartUpdateAsync()
    {
        await new CoreLib.udp.UdpClient().SendBroadcastMessageAsync(
            new BroadcastMessage { MachineName = Environment.MachineName, Command = Command.StartUpdate });
    }

    protected Action _onConfirmeUpdate { get; set; }
    public UpdaterServer OnConfirmUpdate(Action action)
    {
        _onConfirmeUpdate = action;
        return this;
    }


}
