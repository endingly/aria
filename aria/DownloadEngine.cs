using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace aria
{
    class DownloadEngine
    {
        private List<Socket> rsockets;
        private List<Socket> wsockets;

        public bool noWait;
        public Queue<Command> commands;
        
    }
}
