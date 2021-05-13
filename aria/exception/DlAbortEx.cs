using System;
using System.Collections.Generic;
using System.Text;

namespace aria.exception
{
    [Serializable]
    public class DlAbortEx : ariaException
    {
        public DlAbortEx() { }

        public DlAbortEx(params string[] msgsrc) : base(msgsrc)
        {

        }

        public DlAbortEx(string message) : base(message)
        {
        }

        public DlAbortEx(string message, Exception inner) : base(message, inner) { }
        protected DlAbortEx(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
