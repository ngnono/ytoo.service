using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.fashion.common.message
{
    public abstract class BaseMessage
    {
        private long _messageId = long.Parse(string.Concat(DateTime.UtcNow.Ticks,new Random().Next(100)));

        public virtual long MessageId
        {
            get
            {
                return _messageId;
            }
        }

        public abstract int ActionType { get; }

        public abstract int SourceType { get; set; }
    }
}
