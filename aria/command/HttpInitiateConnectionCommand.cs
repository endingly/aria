using aria.util;
using System;
using System.Collections.Generic;
using System.Text;

namespace aria.command
{
    class HttpInitiateConnectionCommand : AbstractCommand
    {
        private bool useProxy()
        {
            return e.option.ContainsKey("http_proxy_enabled") && e.option["http_proxy_enabled"] == "true";
        }

        override public bool executeInternal(Segment segment)
        {
            Command command = new Command();
            if (useProxy())
            {
                e.logger.Info(Message.MSG_CONNECTING_TO_SERVER,
                    cuid.ToString(),
                    e.option["http_proxy_host"],
                    e.option["http_proxy_port"]
                    );
                socket.Connect(e.option["http_proxy_host"], int.Parse(e.option["http_proxy_port"]));
            }
            e.commands.Enqueue(command);
            return true;
        }

        public HttpInitiateConnectionCommand(int cuid, ref Request req, ref DownloadEngine e)
        {
            this.cuid = cuid;
            this.req = req;
            this.e = e;
        }

        public HttpInitiateConnectionCommand()
        {

        }

    }
}
