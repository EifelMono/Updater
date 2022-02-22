namespace Updater.CoreLib.udp;

public class UdpServer : UdpCore, IDisposable
{
    private CancellationTokenSource CancellationTokenSource { get; set; }
    public void Dispose()
    {
        CancellationTokenSource.Cancel();
    }

    public async Task RunAsync(Action<string> action, CancellationTokenSource cancellationTokenSource)
    {
        CancellationTokenSource = cancellationTokenSource;
        try
        {
            using var _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Any, Globals.UDPPort));

            State state = new State();
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var bytes = await _socket.ReceiveAsync(state.buffer, SocketFlags.None, cancellationTokenSource.Token);
                var text = Encoding.ASCII.GetString(state.buffer, 0, bytes);
                _ = Task.Run(() => action?.Invoke(text));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}
