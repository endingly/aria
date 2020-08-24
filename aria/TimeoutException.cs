using System;
using System.Collections.Generic;
using System.Text;

namespace aria
{
    class TimeoutException : Exception
    {
        public int err;
        public string msg;

        public TimeoutException()
        {
            err = 0;
            msg = " ";
        }

        public TimeoutException(int err)
        {
            this.err = err;
            msg = " ";
        }

        public TimeoutException(string msg)
        {
            err = 0;
            this.msg = msg;
        }

        public TimeoutException(int err,string msg)
        {
            this.err = err;
            this.msg = msg;
        }
    }
}
