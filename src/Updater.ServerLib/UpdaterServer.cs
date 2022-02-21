using Grpc.Core;
using Updater.CoreLib;
using Updater.CoreLib.grpc;
using Updater.CoreLib.udp;

namespace Updater.ServerLib;

class GreeterImpl : Greeter.GreeterBase
{
    // Server side handler of the SayHello RPC
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        Console.WriteLine(request);
        return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
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
            Services = { Greeter.BindService(new GreeterImpl()) },
            Ports = { new ServerPort("0.0.0.0", Globals.grpcPort, ServerCredentials.Insecure) }
        };
        GrpcServer.Start();
    }
    public async void Dispose()
    {
        CancellationTokenSource?.Cancel();
        await GrpcServer.ShutdownAsync();
    }

    public async Task NotifyUpdaterAvailableAsync()
    {
        await new CoreLib.udp.UdpClient().SendAsync(Environment.MachineName);
    }

    public Task NotifyUpdateAvailableAsync()
    {
        return Task.CompletedTask;
    }

    protected Action _onStartUpdate { get; set; }
    public UpdaterServer OnStartUpdate(Action action)
    {
        _onStartUpdate = action;
        return this;
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
