using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler
{
    public abstract class MessageHandler
    {
        public abstract int SourceType { get; }

        public abstract int ActionType { get; }

        public virtual bool Work(fashion.common.message.BaseMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
