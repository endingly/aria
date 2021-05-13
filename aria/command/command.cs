using System;
using System.Collections.Generic;
using System.Text;

namespace aria.command
{
    class Command
    {
        protected int cuid { get; set; }

        public Command(int cuid)
        {
            this.cuid = cuid;
        }

        public Command()
        {

        }

        public virtual bool execute()
        {
            return true;
        }
    }
}
