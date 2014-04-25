using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.common.Tencent
{
    public class AutoBankTransfer
    {
        private static object lockObject = new object();
        private static AutoBankTransfer transferObject = null;
        private string operateCode = string.Empty;
        private string operateName = string.Empty;
        private string operateUserId = string.Empty;
        private string operateUserPwd = string.Empty;
        private string parterId = string.Empty;
        private string caFilePath = string.Empty;
        private string certFilePath = string.Empty;
        private string staticIp;
        private StringBuilder debugInfo = new StringBuilder();
        private AutoBankTransfer()
        {
            operateCode = Config.OP_CODE;
            operateName = Config.OP_NAME;
            operateUserId = Config.OP_USERID;
            operateUserPwd = Config.OP_USERPWD;
            parterId = Config.PARTER_ID;
            caFilePath = Config.CA_FILE_PATH;
            certFilePath = Config.CERT_FILE_PATH;
            staticIp = Config.STATIC_PUBLIC_IP;
        }

        public static AutoBankTransfer Instance { get {
            if (transferObject == null)
            {
                lock (lockObject)
                {
                    if (transferObject == null)
                    {
                        transferObject = new AutoBankTransfer();
                    }
                }
            }
            return transferObject;
        } }

        public BatchTransferResponse BatchTransfer(BatchTransferRequest request)
        {
            EnsureRequest(request);
            return Send(request) as BatchTransferResponse;
        }

        public BatchQueryResponse Query(BatchQueryRequest request)
        {
            EnsureRequest(request);
            return Send(request) as BatchQueryResponse;
        }


        private BaseBatchResponse Send(BaseBatchRequest request)
        {
            var client = WebRequest.CreateHttp(Config.SERVICE_URI_BATCH_TRANSFER);
            client.ContentType = "application/x-www-form-urlencoded";
            client.Method = "Post";
            using (var webRequest = client.GetRequestStream())
            using (var streamWriter = new StreamWriter(webRequest, Config.DEFAULT_ENCODE))
            {
                string requestStr = request.EncodedFormat;

                streamWriter.Write(requestStr);
            }
            StringBuilder sb = new StringBuilder();
            using (var response = client.GetResponse())
            {
                using (var body = response.GetResponseStream())
                {
                    var serializer = new XmlSerializer(request.GetType());

                    var transferResponse = serializer.Deserialize(body) as BatchQueryResponse;
                    if (transferResponse == null)
                    {
                        debugInfo.AppendLine(string.Format("batch send response fail, raw response:{0}", body));
                        return null;
                    }
                    return transferResponse;
                }

            }
        }

        private void EnsureRequest(BatchTransferRequest request)
        {
            if (request != null) {
                request.OperateCode = operateCode;
                request.OperateName = operateName;
                request.OperatePwdMd5 = operateUserPwd;
                request.OperateUser = operateUserId;
                request.SPId = parterId;
                request.OperateTime = DateTime.Now.ToString("yyyyMMddHHmmssSSS");
                request.ClientIp = staticIp;
            }
            
        }
        private void EnsureRequest(BatchQueryRequest request)
        {
            if (request != null)
            {
                request.OperateCode = operateCode;
                request.OperateName = operateName;
                request.OperatePwdMd5 = operateUserPwd;
                request.OperateUser = operateUserId;
                request.SPId = parterId;
                request.OperateTime = DateTime.Now.ToString("yyyyMMddHHmmssSSS");
                request.ClientIp = staticIp;
            }

        }
        
    }
}
