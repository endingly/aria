using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace aria
{
    class AbstractCommand : Command
    {
        private DateTime checkPoint;
        protected Request req;
        protected Socket socket;
        protected bool checkSocketIsReadable;
        protected bool checkSocketIsWritable;

        public AbstractCommand(int cuid,ref Request req,,Socket s=null)
        {

        }
    }
}
