using System;
using System.Collections.Generic;
using System.Text;

namespace aria.exception
{
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

        public ariaException(string message) : base(message)
        {
        }

        public ariaException(string message, Exception inner) : base(message, inner) { }
        protected ariaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
