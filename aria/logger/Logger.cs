using System;
using System.Collections.Generic;
using System.Text;

namespace aria.logger
{
    class Logger
    {
        public virtual void Debug(params string[] msg)
        {
            return;
        }

        public virtual void Debug(Exception ex, params string[] msg)
        {
            return;
        }

        public virtual void Info(params string[] msg)
        {
            return;
        }

        public virtual void Info(Exception ex, params string[] msg)
        {
            return;
        }

        public virtual void Error(params string[] msg)
        {
            return;
        }

        public virtual void Error(Exception ex, params string[] msg)
        {
            return;
        }
    }
}
