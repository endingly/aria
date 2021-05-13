using System;
using System.Collections.Generic;
using System.Text;

namespace aria.exception
{
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
}
