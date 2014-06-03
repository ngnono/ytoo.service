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
        private ConnectionFactory _factory = new ConnectionFactory() { HostName = RabbitClientConfiguration.Current.Host };
 
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

        public void ReceiveReliable<TMessage>(Func<TMessage,bool> postMessageHandler) where TMessage:BaseMessage
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

                    while (true)
                    {
                        var ea =
                            (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        var body = ea.Body;
                        var rawMessage = Encoding.UTF8.GetString(body);
                        var message = JsonConvert.DeserializeObject(rawMessage) as TMessage;
                        if (postMessageHandler(message))
                            channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

       

    }
}
