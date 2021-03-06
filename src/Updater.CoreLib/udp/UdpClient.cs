namespace Updater.CoreLib.udp;

public class UdpClient : UdpCore
{
    public Task SendBroadcastMessageAsync(BroadcastMessage message)
    {
        try
        {
            using var _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(IPAddress.Broadcast, Globals.UDPPort);
            State state = new State();
            EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);

            byte[] data = Encoding.ASCII.GetBytes(message.ToJson());
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
            }, state);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
        return Task.CompletedTask;
    }
}
