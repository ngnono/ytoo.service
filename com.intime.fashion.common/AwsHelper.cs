using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.common
{
    public static class AwsHelper
    {
        private static AmazonSQS sqs;
        private static string queueUrl;
        private static object lockObject = new object();
        public static void SendMessage(string typeName,Func<object> messageComposer)
        {
           
            Task.Factory.StartNew(() => {
                EnsureQueue();
                Logger.Debug("starting sync message...");
                SendMessageRequest sendMessageRequest = new SendMessageRequest();
                sendMessageRequest.QueueUrl = queueUrl;
                sendMessageRequest.MessageBody = JsonConvert.SerializeObject(new { type = typeName, data = messageComposer() });

                sqs.SendMessage(sendMessageRequest);
                Logger.Debug("completed one message sync");
            });
           
        }

        private static void EnsureQueue()
        {
            if (!string.IsNullOrEmpty(queueUrl))
                return;
            EnsureSqs();
            lock (lockObject)
            {
                CreateQueueRequest sqsRequest = new CreateQueueRequest();
                sqsRequest.QueueName = ConfigurationManager.AppSettings["awssqsqueue"] ;
                if (sqsRequest.QueueName == null)
                {
                    Logger.Info(string.Format("awssqsqueue not configured, configure it first!"));
                    return;
                }
                CreateQueueResponse createQueueResponse = sqs.CreateQueue(sqsRequest);
                queueUrl = createQueueResponse.CreateQueueResult.QueueUrl;
            }
        }

        private static void EnsureSqs()
        {
            if (sqs == null)
            {
                lock (lockObject)
                {
                    sqs = AWSClientFactory.CreateAmazonSQSClient();

                }
            }
        }
        private static ILog Logger
        {
            get {
                return ServiceLocator.Current.Resolve<ILog>();
            }
        }
    }
}
