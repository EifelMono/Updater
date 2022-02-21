using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater.CoreLib
{
    public static class Globals
    {
        public static int UDPPort { get; private set; } = 10001;
        public static int grpcPort { get; private set; } = 10002;

    }
}
