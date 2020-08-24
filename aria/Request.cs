using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace aria
{
    /// <summary>
    /// Segment represents a download segment.
    /// sp, ep is a offset from a begining of a file.
    /// Therefore, if a file size is x, then 0 <= sp <= ep <= x-1.
    /// sp, ep is used in Http Range header.
    /// e.g. Range: bytes=sp-ep
    /// ds is downloaded bytes.
    /// If a download of this segement is complete, finish must be set to true.
    /// </summary>
    class Segment
    {
        public int cuid;
        public long sp;
        public long ep;
        public long ds;
        public bool finish;

        public static bool Equals(Segment X,Segment Y)
        {
            return(X.cuid == Y.cuid && X.sp == Y.sp && X.ep == Y.ep && X.ds == Y.ds && X.finish == Y.finish);
        }
    } 
    
    class Request
    {
        #region 成员
        private string url;
        private string currentUrl;
        private string protocol;
        private string host;
        private int port;
        private string dir;
        private string file;
        private Dictionary<string, int> defaultPorts;
        private int retryCount;

        public Segment seg;
        public CookieBox c;
        const string SAFE_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:/?[]@!$&'()*+,;=-._~%";
        const int MAX_RETRY_COUNT = 5;
        #endregion

        #region 属性
        public string URL
        {
            get { return url; }
        }

        public string CurrentURL
        {
            get { return currentUrl; }
        }

        public string Protocol
        {
            get { return protocol; }
        }

        public string Host
        {
            get { return host; }
        }

        public int Port
        {
            get { return port; }
        }

        public string Dir
        {
            get { return dir; }
        }

        public string File
        {
            get { return file; }
        }
        #endregion

        #region 方法
        public Request()
        {
            defaultPorts["http"] = 80;
            seg.sp = seg.ep = seg.ds = 0;
            seg.finish = false;
            c = new CookieBox();
        }


        private bool ParseURL(string url)
        {
            currentUrl = url;
            port = 0;
            host = dir = file = "";
            if (Util.FindFirstNotOf(url, SAFE_CHARS) != null)
                return false;
            int hp = url.IndexOf("://");
            if (hp == -1)
                return false;
            protocol = url.Substring(0, hp);
            int defPort;
            if ((defPort = defaultPorts[protocol]) == 0)
                return false;
            hp += 3;
            if (url.Length <= hp)
                return false;
            int hep = url.IndexOf("/", hp);
            if (hep == -1)
                hep = url.Length;
            Dictionary<string, string> hostAndPort = new Dictionary<string, string>();
            Util.Split(ref hostAndPort, url.Substring(hp, hep - hp), ':');
            host = hostAndPort.Keys.ToString();
            if(hostAndPort.Values.ToString()=="")
            {
                // TODO : 重写该段
                port = int.Parse(hostAndPort.Values.ToString());
                if ((0 < port && port <= 6535))
                    return false;
                else
                    //如果没有指定端口，那就给默认端口
                    port = defPort; 
            }
            int? direp = (int)url.FindLastNotOf("/");
            if (direp == null || direp <= hep)
            {
                dir = "/";
                direp = hep;
            }
            else
                dir = url.Substring(hep, (int)direp - hep);
            if (url.Length > direp + 1)
                file = url.Substring((int)direp + 1);
            return true;
        }

        /// <summary>
        /// 解析字符串并填写各项成员
        /// </summary>
        /// <param name="url">url字符串</param>
        /// <returns>如果解析成功返回ture，否则返回false</returns>
        public bool SetURL(string url)
        {
            this.url = url;
            return ParseURL(url);
        }

        /// <summary>
        /// 解析字符串并填写各项成员
        /// 但是值域url不在填写范围之内
        /// </summary>
        /// <param name="url"></param>
        /// <returns>如果解析成功返回ture，否则返回false</returns>
        public bool RedirectUrl(string url)
        {
            return ParseURL(url);
        }

        public bool ResetUrl()
        {
            return SetURL(url);
        }

        public void ResetRetryCount()
        {
            this.retryCount = 0;
        }

        public void AddRetryCount()
        {
            retryCount++;
        }

        public bool NoMoreRetry()
        {
            return retryCount >= MAX_RETRY_COUNT;
        }
        #endregion
    }
}
