using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.fashion.common.message
{
    public interface IMessageReceiver:IDebugInfo
    {
        void ReceiveReliable(Func<BaseMessage,bool> postMessageHandler);

        void Cancel();
    }
}
