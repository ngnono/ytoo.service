using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SQS;
using Amazon.SQS.Model;
using com.intime.fashion.common.Aws;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.common
{
    public static class AwsHelper
    {
        private static AmazonSQS sqs;
        private static string queueUrl;
        private static object lockObject = new object();
        private static AmazonS3 s3;
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

        public static bool SendHttpMessage(string url, object requestData, string publickey, string privatekey, Action<dynamic> successCallback,Action<dynamic> failCallback)
        {
            
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url is empty");
            if (string.IsNullOrEmpty(publickey) || string.IsNullOrEmpty(privatekey))
                throw new ArgumentException("private/public key is empty");

            var requestbody =JsonConvert.SerializeObject(new
            {
                data = requestData
            });
            string requestUrl = ConstructAwsHttpRequestUrl(url, publickey, privatekey);
            var client = WebRequest.CreateHttp(requestUrl);
            client.ContentType="application/json";
            client.Method = "Post";
            using(var request = client.GetRequestStream())
            using (var streamWriter = new StreamWriter(request))
            {
                streamWriter.Write(requestbody);
            }
            StringBuilder sb = new StringBuilder();
            using (var response = client.GetResponse())
            {
                var body = response.GetResponseStream();
                using (var streamReader = new StreamReader(body, Encoding.UTF8))
                {

                    sb.Append(streamReader.ReadToEnd());
                 
                }
            }
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(sb.ToString());

            if (jsonResponse != null &&
                jsonResponse.isSuccessful == true)
            {
                if (successCallback != null) 
                successCallback(jsonResponse);
                return true;
            }
            else
            {
                if (failCallback != null)
                failCallback(jsonResponse);
                return false;
            }


        }

        public static void Transfer2S3(string localPath,string remotePath)
        {
            EnsureS3();
            var transferClient = new TransferUtility(s3);
            transferClient.Upload(new TransferUtilityUploadRequest() { 
                 CannedACL = Amazon.S3.Model.S3CannedACL.PublicRead,
                 FilePath = localPath,
                 BucketName = AwsConfig.S3_BUCKET_NAME,
                 Key = remotePath
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
        private static void EnsureS3()
        {
            if (s3 == null)
            {
                lock (lockObject)
                {
                    s3 = AWSClientFactory.CreateAmazonS3Client(new AmazonS3Config() {
                        ServiceURL = "s3.amazonaws.com",
                        CommunicationProtocol = Amazon.S3.Model.Protocol.HTTP
                    });

                }
            }
        }
        private static ILog Logger
        {
            get {
                return ServiceLocator.Current.Resolve<ILog>();
            }
        }

        private static string ConstructAwsHttpRequestUrl(string host,string publickey,string privatekey)
        {
            
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("key", publickey);
            query.Add("nonce", new Random(1000).Next().ToString());
            query.Add("timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"));
            var signingValue = new StringBuilder();
            var signedValue = string.Empty;
            foreach (var s in query.Values.ToArray().OrderBy(s => s))
                signingValue.Append(s);
            Logger.Debug(string.Format("signed value:{0}", signingValue.ToString()));
            using (HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(privatekey)))
            {
                var hashValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(signingValue.ToString()));
                signedValue = Convert.ToBase64String(hashValue);
                Logger.Debug(signedValue);
               
            }
            query.Add("sign", signedValue);
            var requestUrl = new StringBuilder();
            requestUrl.Append(host);
            requestUrl.Append("?");
            return query.Keys.Aggregate(requestUrl, (s, e) => s.AppendFormat("&{0}={1}", e, HttpUtility.UrlEncode(query[e])), s => s.ToString());

        }
    }
}
