using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace aria
{
    class DownloadEngine
    {
        private List<Socket> rsockets;
        private List<Socket> wsockets;

        public bool noWait;
        public Queue<Command> commands;
        public SegmentMan segmentMan;
        public DiskWriter diskWriter;
        public Logger logger;
        public Dictionary<string, string> option;


        public DownloadEngine()
        {
            noWait = false;
        }

        ~DownloadEngine()
        {
            if (rsockets.Count==0)
            {
                logger.Info("read socket is empty!");
            }
            if (wsockets.Count == 0)
            {
                logger.Info("writer socket is empty!");
            }
        }

        // TODO : 将要补充
        private void waitDate()
        {

        }

        private bool addSocket(ref List<Socket> sockets, Socket socket)
        {
            if (sockets.IndexOf(socket) == 0)
            {
                sockets.Add(socket);
                return true;
            }
            else
                return false;
        }

        private bool deleteSocket(ref List<Socket> sockets, Socket socket)
        {
            if (sockets.IndexOf(socket) == 0)
            {
                sockets.Remove(socket);
                return true;
            }
            else
                return false;
        }

        public void run()
        {
            TimeSpan cp = new TimeSpan();
            int speed = 0;
            long psize = 0;
            while (commands.Count!=0)
            {
                for(int i=0;i<commands.Count;i++)
                {
                    Command com = commands.Peek();
                    commands.Dequeue();    
                }
                if (!noWait&&commands.Count!=0)
                {
                    waitDate();
                    long dlSize = segmentMan.GetDownLoadedSize();
                    TimeSpan now = new TimeSpan();
                    if (cp.TotalSeconds==0&&cp.TotalMilliseconds==0)
                    {
                        cp = now;
                        psize = dlSize;
                    }
                    else
                    {
                        long elapsed = Util.Difftv(now, cp);
                        if (elapsed>=5000000)
                        {
                            int nspeed = (int)((dlSize - psize) / (elapsed / 1000000.0));
                            speed = (nspeed + speed) / 2;
                            cp = now;
                            psize = dlSize;
                            Console.WriteLine("\r                                                                            ");
                            Console.WriteLine("\rProgress {0} Bytes/ {1} Bytes {2} KB/s {3}% ({4} connections)",
                                Util.Llitos(dlSize,true),
                                Util.Llitos(segmentMan.totalSize,true),
                                segmentMan.totalSize==0?0:(dlSize*100)/segmentMan.totalSize,
                                speed/1000.0,
                                commands.Count);
                        }
                    }
                }
                noWait = false;
            }
            segmentMan.RemoveIfFinished();
            diskWriter.closeFile();
            if (segmentMan.Finished())
                Console.WriteLine("\nThe download was complete. <" + segmentMan.FilePath);
            else
                Console.WriteLine("\nThe download was not complete because of errors. Check the log.");
        }

        public bool addSocketForReadCheck(ref Socket socket)
        {
            return addSocket(ref rsockets, socket);
        }

        public bool deleteSocketForReadCheck(Socket socket)
        {
            return deleteSocket(ref rsockets, socket);
        }

        public bool addSocketForWriteCheck(Socket socket)
        {
            return addSocket(ref wsockets, socket);
        }

        public bool deleteSocketForWriteCheck(Socket socket)
        {
            return deleteSocket(ref wsockets, socket);
        }
    }
}
