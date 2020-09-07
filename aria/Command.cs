using aria.download;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;

namespace aria.command
{
    class Command
    {
        protected int cuid { get; set; }

        public Command(int cuid)
        {
            this.cuid = cuid;
        }

        public Command()
        {

        }

        public virtual bool execute()
        {
            return true;
        }
    }

    class AbstractCommand : Command
    {
        private TimeSpan checkPoint;
        protected Request req;
        protected Socket socket;
        protected DownloadEngine e;
        protected bool checkSocketIsReadable;
        protected bool checkSocketIsWritable;
        const int TIMEOUT_SEC = 5;

        private void updateCheckPoint()
        {
            checkPoint = DateTime.Now - DateTime.Today;
        }

        private bool isTimeoutDetected()
        {
            TimeSpan now = DateTime.Now - DateTime.Today;
            if (checkPoint == TimeSpan.Zero)
                return false;
            else
            {
                long elapsed = Util.Difftv(now, checkPoint);
                if (elapsed >= TIMEOUT_SEC * 1000)  // 5000毫秒
                    return true;
                else
                    return false;
            }
        }

        public AbstractCommand(int cuid,  Request req,  DownloadEngine e, Socket s = null)
        {
            this.cuid = cuid;
            this.req = req;
            checkSocketIsReadable = checkSocketIsWritable = false;
            this.e = e;
            this.socket = s;
            this.checkPoint = TimeSpan.Zero;
        }

        public AbstractCommand()
        {

        }

        ~AbstractCommand()
        {
            e.deleteSocketForReadCheck(socket);
            e.deleteSocketForWriteCheck(socket);
        }

        private void Command(int cuid)
        {
            throw new NotImplementedException();
        }

        public virtual bool prepareForRetry()
        {
            e.commands.Enqueue()
        }

        public virtual void onError(Exception e)
        {

        }

        public virtual bool executeInternal(Segment segment)
        {
            return false;
        }

        public bool execute()
        {
            try
            {
                if (checkSocketIsReadable && !socket.Poll(0, SelectMode.SelectRead)
                    || checkSocketIsWritable && !socket.Poll(0, SelectMode.SelectWrite))
                {
                    updateCheckPoint();
                    e.commands.Enqueue(this);
                    return false;
                }
                if (isTimeoutDetected())
                    throw new DlRetryEx(Message.EX_TIME_OUT);
                updateCheckPoint();
                Segment seg = new Segment();
                seg.cuid = 0;
                seg.ds = seg.ep = seg.sp = 0;
                seg.finish = false;
                if (e.segmentMan.downloadStarted)
                {
                    if (!e.segmentMan.getSegment(ref seg, cuid))
                    {
                        e.logger.Info(Message.MSG_NO_SEGMENT_AVAILABLE, cuid);
                        return true;
                    }
                }
                return executeInternal(seg);
            }
            catch (DlRetryEx err)
            {
                e.logger.Error(Message.MSG_RESTARTING_DOWNLOAD, err.ToString());
                onError(err);
                req.ResetUrl();
                req.AddRetryCount();
                if (req.NoMoreRetry())
                {
                    e.logger.Error(Message.MSG_MAX_RETRY);
                    return true;
                }
                else
                    return prepareForRetry();
            }
        }
    }

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

    class HttpProxyRequestCommand : AbstractCommand
    {
        override public bool executeInternal(Segment segment)
        {
            socket.Blocking = false;
            HttpMessageHandler httpMessageHandler;
            
        }

        public HttpProxyRequestCommand(int cuid, ref Request req,ref DownloadEngine e,ref Socket s)
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

    class DownloadCommand : AbstractCommand
    {
        public string transferEncoding;

        protected bool executeInternal(Segment segment)
        {
            TransferEncoding? te = new TransferEncoding?();
            if (transferEncoding.Length!=0)
                te = getTransferEncoding(transferEncoding);
            int bufSize = 4096;
            byte[] buf = new byte[bufSize];
            socket.Receive(buf);
            if (te!=null)
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

        public DownloadCommand(int cuid, Request req, DownloadEngine e, Socket s):base(cuid, req, e, s)
        {
            
        }

        public DownloadCommand() { }

        virtual TransferEncoding getTransferEncoding(string transferEncoding)
        {

        }

    }
}
