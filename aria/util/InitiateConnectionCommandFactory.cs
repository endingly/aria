using aria.command;
using System;
using System.Collections.Generic;
using System.Text;

namespace aria.util
{
    class InitiateConnectionCommandFactory
    {
        public static Command createInitiateConnectionCommand(int cuid,ref Request req,ref DownloadEngine e)
        {
            if (req.Protocol == "http")
                return new HttpInitiateConnectionCommand(cuid, req, e);
            else
                //这个协议目前还不支持
                return null;
        }
    }
}
