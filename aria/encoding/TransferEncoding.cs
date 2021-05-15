using System;
using System.Collections.Generic;
using System.Text;

namespace aria.encoding
{
    class TransferEncoding
    {
        public virtual void Init() 
        { 

        }

        /// <summary>
        /// 这个函数对缓冲区的码流进行格式变换
        /// 原型是：virtual void inflate(char* outbuf, int& outlen, const char* inbuf, int inlen)
        /// 这里删除了中间不必要的参数
        /// </summary>
        /// <param name="outbuf">返回的缓冲</param>
        /// <param name="inbuf">输入的缓冲区</param>
        /// <param name="inlen">输入缓冲区的长度</param>
        public virtual void Inflate(ref char[] outbuf, ref int outlen, ref char[] inbuf, int inlen)
        {
            outbuf = new char[] { '-', '-' };
        }

        public virtual bool Finished()
        {
            return true;
        }

        public virtual void End()
        {

        }

    }
}
