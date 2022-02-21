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
            EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
            AsyncCallback recv = null;
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                _socket.BeginReceiveFrom(state.buffer, 0, State.bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                    _socket.BeginReceiveFrom(so.buffer, 0, State.bufSize, SocketFlags.None, ref epFrom, recv, so);
                    var text = Encoding.ASCII.GetString(so.buffer, 0, bytes);
                    action?.Invoke(text);
                }, state);
                await Task.Delay(10);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}
