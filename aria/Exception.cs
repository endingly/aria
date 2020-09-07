using System;
using System.Collections.Generic;
using System.Text;

namespace aria.exception
{

    [Serializable]
    public class SocketIOException : ariaException
    {
        public SocketIOException() { }
        public SocketIOException(string message) : base(message) { }
        public SocketIOException(string message, Exception inner) : base(message, inner) { }
        protected SocketIOException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    class TimeoutException : ariaException
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

        public TimeoutException(int err, string msg)
        {
            this.err = err;
            this.msg = msg;
        }
    }


    [Serializable]
    public class DlRetryEx : ariaException
    {
        public DlRetryEx() { }
        public DlRetryEx(params string[] msgsrc) : base(msgsrc) { }
        public DlRetryEx(string message) : base(message) { }
        public DlRetryEx(string message, Exception inner) : base(message, inner) { }
        protected DlRetryEx(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class DlAbortEx : ariaException
    {
        public DlAbortEx() { }

        public DlAbortEx(params string[] msgsrc) :base(msgsrc)
        {
            
        }

        public DlAbortEx(string message)  :base(message)
        {  
        }

        public DlAbortEx(string message, Exception inner) : base(message, inner) { }
        protected DlAbortEx(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class ariaException : Exception
    {
        public new string Message
        {
            get;
            set;
        }

        public ariaException(params string[] msgsrc)
        {
            foreach (string item in msgsrc)
                this.Message += item;
        }

        public ariaException() { }

        public ariaException(string message) :base(message)
        {
        }

        public ariaException(string message, Exception inner) : base(message, inner) { }
        protected ariaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
