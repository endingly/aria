using System;
using System.Collections;
using aria.exception;
using aria.util;

namespace aria.encoding
{

    internal enum STATE
    {
        READ_SIZE,
        READ_DATA,
        FINISH
    }

    class ChunkedEncoding : TransferEncoding
    {
        long chunkSize;
        STATE state;
        char[] strbuf;
        int strbufSize;
        const int MAX_BUFSIZE = 8192;

        /// <summary>
        /// strbuf 以 NULL 为结尾，而且 inlen=strbuf.Length
        /// 所以，strbuf[inlen] = '\0'
        /// 此处对原来的形参列表做了修改
        /// </summary>
        /// <param name="pp">指定需要检查的字符串数组</param>
        /// <param name="index">下标</param>
        /// <returns></returns>
        int ReadChunkSize(char[] pp, ref int index)
        {
            // 从数组中读取消息
            string str = pp.ToString();
            var strIndex = str.IndexOf("\r\n");
            if (strIndex == -1)
            {
                // 如果字符不满足要求，那就直接返回
                return -1;
            }
            // 忽略 chunk-extension
            var strIndex_exsp = str.IndexOf(';');
            if (strIndex_exsp == -1)
            {
                strIndex_exsp = strIndex;
            }
            // TODO: 检查字符串中无效的字符
            chunkSize = Convert.ToInt64(str, 16);
            if (chunkSize < 0)
            {
                throw new DlAbortEx(Message.EX_INVALID_CHUNK_SIZE);
            }
            else if ((chunkSize == long.MaxValue || chunkSize == long.MinValue))
            {
                throw new DlAbortEx(Message.EX_LIMIT_OVERFLOW);
            }
            index += 2;
            return 0;
        }

        int ReadData(in char[] pp,ref char[] buf,ref int len, int maxlen,ref int index)
        {
            if (buf.Length + len == buf.Length + maxlen)
            {
                return -1;
            }
            if (chunkSize == 0)
            {
                return ReadDataEOL(pp, ref index);
            }
            int wsize;
            if (pp.Length < chunkSize)
            {
                wsize = pp.Length <= maxlen - len ? pp.Length : maxlen - len;
            }
            else
            {
                wsize = chunkSize <= maxlen - len ? (int)chunkSize : maxlen - len;
            }
            pp.AsSpan(0, wsize).ToArray().CopyTo(buf, len);
            chunkSize -= wsize;
            len += wsize;
            index += wsize;
            if (chunkSize == 0)
            {
                return ReadDataEOL(pp,ref index);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 添加数组的长度,并将多余的数据放入缓冲区
        /// </summary>
        /// <param name="inbuf">输入的缓冲区</param>
        /// <param name="inlen">此缓冲区的长度</param>
        void addBuffer(in char[] inbuf, int inlen)
        {
            if (strbuf.Length + inlen >= strbufSize)
            {
                if (strbuf.Length + inlen + 1 > MAX_BUFSIZE)
                {
                    throw new DlAbortEx(Message.EX_TOO_LARGE_CHUNK, (strbuf.Length + inlen + 1).ToString());
                }
                char[] temp = new char[strbuf.Length + inlen + 1];
                inbuf.CopyTo(temp, strbuf.Length + 1);
                strbuf = temp;
                strbufSize = strbuf.Length;
            }
            int origlen = strbuf.Length;
            inbuf.CopyTo(strbuf, origlen);
            strbuf[origlen + inlen] = '\0';
        }

        /// <summary>
        /// 原函数由二重指针写就，这里略作删改，调用方式也会随之变化
        /// </summary>
        /// <param name="p">指定需要检查的字符串数组</param>
        /// <param name="charEnumerator">该字符串数组的迭代器</param>
        /// <returns>0 表示成功，否则即为失败</returns>
        int ReadDataEOL(in char[] p, ref int charEnumerator)
        {
            var str = p.ToString();
            if (str == "\r\n")
            {
                charEnumerator += 2;
                return 0;
            }
            else if (str.Length < 2)
            {
                return -1;
            }
            else
            {
                throw new DlAbortEx(Message.EX_INVALID_CHUNK_SIZE);
            }


        }

        public ChunkedEncoding()
        {
            strbufSize = 4096;
            strbuf = new char[strbufSize];
            strbuf[0] = '\0';
            state = (int)STATE.READ_SIZE;
            chunkSize = 0;
        }

        public override bool Finished()
        {
            return (state == (int)STATE.FINISH) ? true : false;
        }

        public override void Inflate(ref char[] outbuf,ref int outlen, ref char[] inbuf, int inlen)
        {
            addBuffer(inbuf, inlen);

            int clen = 0;
            int index = 0;

            while (true)
            {
                if (state == STATE.READ_SIZE)
                {
                    if (ReadChunkSize(strbuf, ref index) == 0)
                    {
                        if (chunkSize == 0)
                        {
                            state = STATE.FINISH;
                        }
                        else
                        {
                            state = STATE.READ_DATA;
                        }
                    }
                    else
                    {
                        // 分片大小不足以接收
                        break;
                    }
                }
                else if (state == STATE.READ_DATA)
                {
                    if (ReadData(strbuf, ref outbuf,ref clen, outlen,ref index) == 0)
                    {
                        state = STATE.READ_SIZE;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
                if (strbuf.Length <= index)
                {
                    break;
                }
            }
            if (strbuf.Length <= index)
            {
                // 给字符串缓冲区添加 NULL-string
                strbuf[0] = '\0';
            }
            else
            {
                // copy string between [p, strbuf+strlen(strbuf)+1], +1 is for NULL
                // character.
                char[] temp = new char[strbufSize];
                strbuf.CopyTo(temp, strbuf.Length - index + 1);
                strbuf = temp;
            }
            outlen = clen;
        }
    }
}
