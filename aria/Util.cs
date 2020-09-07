using System;
using System.Collections.Generic;

namespace aria
{
    public static class Util
    {
        internal static string Llitos(Int64 value, bool comma = false)
        {
            string str = "#";
            bool flag = false;
            if (value < 0)
            {
                flag = true;
                value = -value;
            }
            else if (value == 0)
            {
                str = "0";
                return str;
            }
            int count = 0;
            while (value > 0)
            {
                ++count;
                string digit = char.ConvertFromUtf32((int)(value % 10 + '0'));
                str = str.Insert(0, digit);
                value /= 10;
                if (comma && count > 3 && count % 3 == 1)
                {
                    str = str.Insert(0, ",");
                }
            }
            if (flag)
            {
                str = str.Insert(0, "-");
            }
            return str;
        }
        internal static string Itos(int value, bool comma)
        {
            string str = Llitos(value, comma);
            return str;
        }

        //用C#对C++标准库中的find_first_not_of函数进行重写
        internal static uint? FindFirstNotOf(this string source, string chars)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (chars == null) throw new ArgumentNullException("chars");
            if (source.Length == 0) return null;
            if (chars.Length == 0) return 0;

            for (int i = 0; i < source.Length; i++)
            {
                if (chars.IndexOf(source[i]) == -1) return (uint?)i;
            }
            return null;
        }

        //用C#对C++标准库中的find_last_not_of函数进行重写
        internal static uint? FindLastNotOf(this string source, string chars)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (chars == null) throw new ArgumentNullException("chars");
            if (source.Length == 0) return null;
            if (chars.Length == 0) return (uint?)(source.Length - 1);

            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (chars.IndexOf(source[i]) == -1) return (uint?)i;
            }
            return null;
        }

        /// <summary>
        /// 对aria基础库中重写的函数
        /// </summary>
        /// <param name="src">需要操作的源对象</param>
        /// <returns>操作后的子字符串</returns>
        internal static string Trim(string src)
        {
            uint? sp = FindFirstNotOf(src, " ");
            uint? ep = FindLastNotOf(src, " ");
            if (sp == null || ep == null)
            {
                return "";
            }
            else
            {
                return src.Substring((int)sp, (int)(ep - sp + 1));
            }
        }

        internal static void Split(ref Dictionary<string, string> hp, string src, char delim)
        {
            hp.Add(" ", " ");
            int p = src.IndexOf(delim);
            if (p == -1)
            {
                hp.Remove(" ");
                hp.Add(src, " ");
            }
            else
            {
                hp.Remove(" ");
                hp.Add(Trim(src.Substring(0, p)), Trim(src.Substring(p + 1)));
            }
        }

        // 重构
        internal static void Split(ref KeyValuePair<string, string> hp, string src, char delim)
        {
            int p = src.IndexOf(delim);
            if (p == -1)
            {
                KeyValuePair<string, string> tmp = new KeyValuePair<string, string>(src, " ");
                hp = tmp;
            }
            else
            {
                KeyValuePair<string, string> tmp = new KeyValuePair<string, string>(Trim(src.Substring(0, p)), Trim(src.Substring(p + 1)));
                hp = tmp;
            }
        }

        /// <summary>
        /// 计算两个参数之间的时间差，以此判断哪个参数较为新
        /// 如果tv1旧于tv2，那么就返回0
        /// </summary>
        /// <param name="tv1"></param>
        /// <param name="tv2"></param>
        /// <returns></returns>
        internal static uint Difftv(TimeSpan tv1, TimeSpan tv2)
        {
            if (tv1 < tv2)
                return 0;
            return (uint)((tv1.TotalSeconds - tv2.TotalSeconds) * 1000 + tv1.TotalMilliseconds - tv2.TotalMilliseconds);
        }

        /// <summary>
        /// 将源字符串src的元数据拆分进result，
        /// result在一开始时是混乱的
        /// </summary>
        /// <param name="result">结果容器</param>
        /// <param name="src">源字符串</param>
        /// <param name="delim">目标字符</param>
        internal static void Slice(ref List<string> result, string src, char delim)
        {
            int p = 0;
            int np = 0;
            while(true)
            {
                np = src.IndexOf(delim, p);
                if(np==-1)
                {
                    if (Trim(src.Substring(p)).Length > 0)
                        result.Add(Trim(src.Substring(p)));
                }
                break;
            }
            string term = src.Substring(p, np - p);
            p = np + 1;
            result.Add(Trim(term));
        }

    }
}
