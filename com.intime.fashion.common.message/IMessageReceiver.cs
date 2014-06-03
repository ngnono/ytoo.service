using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.fashion.common.message
{
    public interface IMessageReceiver
    {
        void ReceiveReliable<TMessage>(Func<TMessage,bool> postMessageHandler) where TMessage :BaseMessage;
    }
}
