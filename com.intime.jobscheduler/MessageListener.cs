using com.intime.fashion.common.message;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.jobscheduler
{
    public class MessageListener
    {
        bool _hasStarted = false;
        IMessageCenterProvider _provider = ServiceLocator.Current.Resolve<IMessageCenterProvider>();
        private ILog _log = LogManager.GetLogger(typeof(MainJobService));
        private static object _lockObject = new object();
        private static MessageListener _instance = new MessageListener();
        private MessageListener()
        {

        }
        public static MessageListener Current
        {
            get
            {
                lock (_lockObject)
                    return _instance;
            }
        }
        public void Start()
        {
            lock (_lockObject)
            {
                if (_hasStarted)
                    return;
                var handlers = GetMessageHandlers();

                _hasStarted = true;
            }
        }

        private IEnumerable<MessageHandler> GetMessageHandlers()
        {
            
        }

        public void Stop()
        {
            if (_hasStarted)
            {

            }
        }
    }
}
