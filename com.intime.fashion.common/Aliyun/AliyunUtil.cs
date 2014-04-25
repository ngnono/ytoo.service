using Aliyun.OpenServices.OpenStorageService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Logger;

namespace com.intime.fashion.common.Aliyun
{
    public static class AliyunUtil
    {
        public static void Transfer2AliyunAsync(string local, string key,Action<object> traceCallBack)
        {
            Task.Factory.StartNew(() =>
            {
                Transfer2Aliyun(local, key,traceCallBack);
            });
        }
        public static bool Transfer2Aliyun(string local, string key,Action<object> traceCallBack )
        {
                try
                {
                    var client = GetOssClient();
                    using (var fs = File.Open(local, FileMode.Open))
                    {
                        client.PutObject(Config.RSS_BUCKET_NAME, key.Replace("\\","/"), fs, new ObjectMetadata());
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    traceCallBack(ex);
                    traceCallBack(string.Format("transfer file:{0} error, need redo it", local));
                    return false;
                }
           
        }

        private static OssClient GetOssClient()
        {
            return new OssClient(Config.ACCESS_ID, Config.ACCESS_KEY);
        }
    }
}
