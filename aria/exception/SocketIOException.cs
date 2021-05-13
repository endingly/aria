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
}
