using System;
using System.Collections.Generic;
using System.Text;

namespace aria.portocol.http
{
    class HttpConnection
    {
        #region member
        private int cuid;
        private Socket socket;
        private readonly Dictionary<string, string> option; // Option
        private Logger logger;
        #endregion

        #region attribute
        private bool UseProxy
        {
            get
            {
                return option.ContainsKey("http_proxy_enabled") && option["http_proxy_enabled"] == "true";
            }
        }

        private bool UseProxyAuth
        {
            get
            {
                return option.ContainsKey("http_proxy_auth_enabled") && option["http_proxy_auth_enabled"] == "true";
            }
        }

        //TODO: Will be rewrite
        private bool UseBasicAuth
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region method

        private string getHost(string host, int port)
        {
            return host + (port == 80 ? "" : ":" + Util.Llitos(port));
        }

        private string createRequest(Request req, Segment segment)
        {
            string request = "GET" + req.CurrentURL + " HTTP/1.1\r\n" +
                "Rrferer: \r\n" +
                "User-Agent: aria\r\n" +
                "Accept:*/*\r\n" +
                "Host: " + getHost(req.Host, req.Port) + "\r\n" +
                "Pragma: no-cache\r\n" +
                "Cache-Control: no-cache\r\n";
            if (segment.sp + segment.ds > 0)
                request += "Range: bytes=" +
                    Util.Llitos(segment.sp + segment.ds) +
                    "-" + Util.Llitos(segment.ep) + "\r\n";
            if (option["http_auth_scheme"] == "BASIC")
                request += "Authorization: Basic " +
                    Base64.encode(option["http_user"] + ":" + option["http_passwd"]) + "\r\n";
            string cookiesValue = string.Empty;
            List<Cookie> cookies = req.c.CriteriaFind(req.Host, req.Dir, req.Protocol == "https" ? true : false);
            foreach (Cookie item in cookies)
                cookiesValue += item.ToString() + ";";
            if (cookiesValue.Length != 0)
                request += "Cookie: " + cookiesValue + "\r\n";
            request += "\r\n";
            return request;
        }

        public HttpConnection()
        {

        }

        public HttpConnection(int cuid, ref Socket socket, Dictionary<string, string> op, ref Logger logger)
        {
            this.cuid = cuid;
            this.socket = socket;
            this.option = op;
            this.logger = logger;
        }

        // Encode default
        public void sendRequest(ref Request req, Segment segment)
        {
            string request = createRequest(req, segment);
            logger.Info(Message.MSG_SENDING_HTTP_REQUEST, cuid.ToString(), request);
            socket.Send(Encoding.Default.GetBytes(request));
        }

        // Encode default
        public void sendProxyRequest(Request req)
        {
            string request = $"CONNECT {req.Host} " +
                $"{Util.Llitos(req.Port)} HTTP/1.1\r\n" +
                $"Host: {getHost(req.Host, req.Port)}\r\n";
            if (this.UseProxyAuth)
                request += "Proxy-Authorization: Basic " +
                    Base64.encode(option["http_proxy_user"] + ":" + option["http_proxy_passwd"]) + "\r\n";
            request += "\r\n";
            logger.Info(Message.MSG_SENDING_HTTP_REQUEST, cuid.ToString(), request);
            socket.Send(Encoding.Default.GetBytes(request));
        }

        public int receiveResponse(ref HttpRequestHeaders headers)
        {
            string header = string.Empty;
            byte[] buf = null;
            try
            {
                // 读取header的一行
                int bufSize = 256;
                // TODO: 限制迭代器的范围
                while (true)
                {
                    bufSize += 256;
                    if (bufSize > 2048)
                        throw new DlAbortEx(Message.EX_INVALID_HEADER);
                    buf = new byte[bufSize];
                    socket.Receive(buf);
                    if (bufSize - 1 > 0)
                        buf[bufSize] = (byte)'\0';
                    header = buf.ToString();
                    if (buf.ToString().Substring(0, 1) == "\r\n")
                        throw new DlAbortEx(Message.EX_NO_HEADER);
                    if (buf.ToString().Contains("\r\n\r\n"))
                    {
                        buf[Array.IndexOf(buf, "\r\n\r\n") + 4] = (byte)'\0';
                        header = buf.ToString();
                        socket.Receive(buf);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            // 拿到所有的头信息
            logger.Info(Message.MSG_RECEIVE_RESPONSE, cuid.ToString(), header);
            int p, np;
            p = np = 0;
            np = header.IndexOf("\r\n", p);
            if (np == -1)
                throw new DlRetryEx(Message.EX_NO_STATUS_HEADER);
            // 检查状态码
            string status = header.Substring(9, 3);
            p = np + 2;
            // 提取键值对
            np = header.IndexOf("\r\n", p);
            while (np != -1 && np != p)
            {
                string line = header.Substring(p, np - p);
                p = np + 2;
                KeyValuePair<string, string> hp = new KeyValuePair<string, string>();
                Util.Split(ref hp, line, ':');
                headers.Add(hp.Key, hp.Value);
            }
            // TODO rewrite this using strtoul
            return int.Parse(status, NumberStyles.Integer);
        }

        #endregion
    }
}
