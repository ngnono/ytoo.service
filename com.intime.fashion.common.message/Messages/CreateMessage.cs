using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.message.Messages
{
   public class CreateMessage:BaseMessage
    {
        public int EntityId { get; set; }
        public override int ActionType
        {
            get { return (int)MessageAction.CreateEntity; }
        }
        public override int SourceType
        {
            get;
            set;
        }
    }
}
