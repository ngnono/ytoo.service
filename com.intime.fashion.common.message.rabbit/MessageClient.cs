using com.intime.fashion.common.config;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.intime.fashion.common.message.rabbit
{
    class MessageClient:IMessageReceiver,IMessageSender
    {
        private ConnectionFactory _factory = new ConnectionFactory() { 
                HostName = RabbitClientConfiguration.Current.Host , 
                UserName = RabbitClientConfiguration.Current.UserName,
                Password = RabbitClientConfiguration.Current.Password};
        private bool _isCancel = false;
        private StringBuilder _debug = new StringBuilder();

        public bool SendMessageReliable(BaseMessage message)
        {
            return SendMessageReliable(message, null);
        }

        public bool SendMessageReliable(BaseMessage message, Action<BaseMessage> preMessageHandler)
        {
            if (preMessageHandler != null)
                preMessageHandler(message);
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var queueName = RabbitClientConfiguration.Current.QueueName;
                    channel.QueueDeclare(queueName, true, false, false, null);

                    var jsonMessage = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(jsonMessage);

                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);
                    channel.BasicPublish("", queueName, properties, body);
                }
            }
            return true;
        }

        public void ReceiveReliable(Func<BaseMessage,bool> postMessageHandler) 
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                   
                    var queueName = RabbitClientConfiguration.Current.QueueName;
                    channel.QueueDeclare(queueName, true, false, false, null);

                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueName, false, consumer);
                    while (!_isCancel)
                    {
                        var ea =
                            (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var rawMessage = Encoding.UTF8.GetString(body);
                        
                        var message = JsonConvert.DeserializeObject<BaseMessage>(rawMessage);
                        if (message == null)
                            _debug.Append(rawMessage);
                        if (postMessageHandler(message))
                            channel.BasicAck(ea.DeliveryTag, false);
                        else
                            channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                }
            }
        }

        public void Cancel()
        {
            _isCancel = true;
           
        }

        public string GetDebugInfo()
        {
            var debugInfo = _debug.ToString();
            _debug.Clear();
            return debugInfo;
        }
    }
}
