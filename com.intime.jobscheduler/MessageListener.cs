using com.intime.fashion.common.message;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.jobscheduler
{
    public class MessageListener
    {
        bool _hasStarted = false;
        IMessageCenterProvider _provider = ServiceLocator.Current.Resolve<IMessageCenterProvider>();
        private ILog _log = LogManager.GetLogger(typeof(MainJobService));
        private IEnumerable<MessageHandler> _handlers = null;
        private CancellationTokenSource _cancelToken = null;
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
                _cancelToken = new CancellationTokenSource();
               _handlers = GetMessageHandlers();
                doStart();
                _hasStarted = true;
                _log.Info("message start listening...");
            }
        }

        private void doStart()
        {
           var task =  Task.Factory.StartNew(() =>
            {
                var receiver = _provider.GetReceiver();
                receiver.ReceiveReliable(message =>
                {
                    try
                    {
                        var validHandlers = _handlers.Where(h => h.SourceType == message.SourceType && (h.ActionType & message.ActionType) == message.ActionType);
                        foreach (var handler in validHandlers)
                        {
                            
                                bool isSuccess = handler.Work(message);
                                if (!isSuccess)
                                {
                                    _log.Warn(string.Format("message not handled:{0}", JsonConvert.SerializeObject(message)));
                                    return false;
                                }
                          
                            
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        _log.Error(receiver.GetDebugInfo()); 
                        return false;
                    }
                });
            },_cancelToken.Token);

        }

        private IEnumerable<MessageHandler> GetMessageHandlers()
        {
            var handlers = new List<MessageHandler>();
            handlers.Add(new Message.ComboHandler());
            return handlers;
        }

        public void Stop()
        {
            if (_hasStarted)
            {
                var receiver = _provider.GetReceiver();
                receiver.Cancel();
                _cancelToken.Cancel();
                _log.Info("message shutdown");
            }
        }
    }
}
