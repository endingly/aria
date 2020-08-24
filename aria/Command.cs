using System;
using System.Collections.Generic;
using System.Text;

namespace aria
{
    class Command
    {
        protected int cuid
        {
            get { return cuid; }
            set { }
        }

        public Command(int cuid)
        {
            this.cuid = cuid;
        }

        public virtual bool execute()
        {
            return true;
        }
    }
}
