namespace Updater.CoreLib.udp;

public class UdpCore
{
    public static int Port => 10001;

    internal class State
    {
        internal static int bufSize = 8 * 1024;
        public byte[] buffer = new byte[bufSize];
    }

}
