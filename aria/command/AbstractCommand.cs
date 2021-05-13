using System;
using System.Collections.Generic;
using System.Text;

namespace aria.command
{
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

        public AbstractCommand(int cuid, Request req, DownloadEngine e, Socket s = null)
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
            e.commands.Enqueue();
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
}
