using System;
using System.Collections.Generic;
using System.Text;

namespace aria.command
{
    class HttpProxyRequestCommand : AbstractCommand
    {
        override public bool executeInternal(Segment segment)
        {
            socket.Blocking = false;
            HttpMessageHandler httpMessageHandler;

        }

        public HttpProxyRequestCommand(int cuid, ref Request req, ref DownloadEngine e, ref Socket s)
        {
            this.cuid = cuid;
            this.req = req;
            this.e = e;
            this.socket = s;
            this.checkSocketIsWritable = true;
            e.deleteSocketForReadCheck(socket);
            e.deleteSocketForWriteCheck(socket);
        }

        public HttpProxyRequestCommand()
        {

        }
    }
}
