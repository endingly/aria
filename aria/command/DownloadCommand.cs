using System;
using System.Collections.Generic;
using System.Text;

namespace aria.command
{
    class DownloadCommand : AbstractCommand
    {
        public string transferEncoding;

        protected bool executeInternal(Segment segment)
        {
            TransferEncoding? te = new TransferEncoding?();
            if (transferEncoding.Length != 0)
                te = getTransferEncoding(transferEncoding);
            int bufSize = 4096;
            byte[] buf = new byte[bufSize];
            socket.Receive(buf);
            if (te != null)
            {
                int infbufSize = 4096;

            }
        }

        protected bool prepareForRetry()
        {

        }

        protected bool prepareForNextSegment()
        {

        }

        public DownloadCommand(int cuid, Request req, DownloadEngine e, Socket s) : base(cuid, req, e, s)
        {

        }

        public DownloadCommand() { }

        virtual TransferEncoding getTransferEncoding(string transferEncoding)
        {

        }

    }
}
