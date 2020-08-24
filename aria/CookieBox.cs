using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Transactions;

namespace aria
{
    class CookieBox
    {
        private List<Cookie> cookies;

        /// <summary>
        /// 设置cookie的各个参数
        /// </summary>
        /// <param name="cookie">目标cookie</param>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        private void SetField(ref Cookie cookie, string name, string value)
        {
            //cookie.Name = cookie.Value = cookie.Expires = cookie.Path = cookie.Domain = "";
            if (name.Equals("secure"))
                cookie.Secure = true;
            else if (name.Equals("domain"))
                cookie.Domain = value;
            else if (name.Equals("path"))
                cookie.Path = value;
            else if (name.Equals("expires"))
                cookie.Expires = DateTime.Parse(value);
            else
            {
                cookie.Name = name;
                cookie.Value = value;
            }
        }

        /// <summary>
        /// 清空私有成员的所有单元
        /// </summary>
        public void Clear()
        {
            this.cookies.Clear();
        }

        /// <summary>
        /// 向cookie列表中添加一个cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void Add(Cookie cookie)
        {
            cookies.Add(cookie);
        }

        /// <summary>
        /// 向cookie列表中添加一个cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void Add(string cookieStr)
        {
            Cookie c = new Cookie();
            Parse(ref c, cookieStr);
            cookies.Add(c);
        }

        /// <summary>
        /// 从一个字符串中解析出完整的cookie信息
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="cookieStr"></param>
        public void Parse(ref Cookie cookie, string cookieStr)
        {
            List<string> terms = new List<string>();
            Util.Slice(ref terms, cookieStr, ';');
            foreach (string itr in terms)
            {
                Dictionary<string, string> nv = new Dictionary<string, string>();
                Util.Split(ref nv, itr, '=');
                SetField(ref cookie, nv.Keys.ToString(), nv.Values.ToString());
            }
        }

        /// <summary>
        /// 在cookie列表中寻找特定的cookie
        /// </summary>
        /// <param name="host">指定的域名</param>
        /// <param name="dir">指定的路径</param>
        /// <param name="secure"></param>
        /// <returns></returns>
        public List<Cookie> CriteriaFind(string host, string dir, bool secure)
        {
            List<Cookie> result = new List<Cookie>();
            foreach (Cookie itr in cookies)
            {
                Cookie c = itr;
                if((secure||!c.Secure&&!secure)&&
                    c.Domain.Length<=host.Length&&
                    c.Path.Length<=dir.Length&&
                    c.Domain.CompareTo(host)==0&&
                    //TODO : we currently ignore expire date.
                    c.Path.CompareTo(dir)==0)
                {
                    result.Add(c);
                }
            }
            return result;
        }
    }
}
