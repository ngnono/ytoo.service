using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Yintai.Architecture.Framework.Extension;
using Yintai.Hangzhou.Service.Contract.Apis;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service.Impl.Apis
{
    internal class GroupCardService : BaseService, IGroupCardService
    {
        private readonly byte[] _key = Encoding.UTF8.GetBytes(ConfigManager.GetGroupCardKey());

        #region methods

        private string GetGroupCardEncryptString(string input)
        {
            return GetBase64String(DesEncrypt(input, _key, CipherMode.ECB));
        }

        private static string GetBase64String(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        private static byte[] DesEncrypt(string input, byte[] key, CipherMode cipherMode)
        {
            var inputByteArray = Encoding.UTF8.GetBytes(input);
            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = key;
                des.Mode = cipherMode;
                using (var mStream = new MemoryStream())
                {
                    using (var cStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();

                        return mStream.ToArray();
                    }
                }
            }
        }

        private static string OpenUrl(string url, string method, string ua, string ct, Dictionary<string, string> queryParams, string enc)
        {
            HttpWebResponse response;
            if (method.Equals("GET"))
            {
                if (queryParams != null)
                {
                    if (url.IndexOf("?", StringComparison.Ordinal) == -1)
                    {
                        url = url + "?";
                    }

                    url = url + EQueryParams(queryParams);
                }
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            if (!String.IsNullOrEmpty(ua))
            {
                request.UserAgent = ua;
            }

            request.Method = method;
            if (!String.IsNullOrEmpty(ct))
            {
                request.ContentType = ct;
            }

            request.ReadWriteTimeout = 10000;

            //if (method.Equals("POST"))
            //{
            //    if (queryParams != null)
            //    {

            //    }
            //}

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                throw;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var body = response.GetResponseStream();

                return Read4Buffer(body, String.IsNullOrEmpty(enc) ? Encoding.UTF8 : Encoding.GetEncoding(enc));
            }

            return null;
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        private static string Read4Buffer(Stream obj, Encoding encoding)
        {
            var reader = new StreamReader(obj, encoding);

            var readbuffer = new char[256];
            var n = reader.Read(readbuffer, 0, 256);
            var sb = new StringBuilder();

            while (n > 0)
            {
                var str = new String(readbuffer, 0, n);
                sb.Append(str);
                n = reader.Read(readbuffer, 0, 256);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        private static string ReadToEnd(Stream obj, Encoding encoding)
        {
            var reader = new StreamReader(obj, encoding);

            var str = reader.ReadToEnd();

            return str;
        }

        private static string EQueryParams(Dictionary<string, string> queryParams)
        {
            if (queryParams == null)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();

            foreach (var key in queryParams.Keys)
            {
                sb.AppendFormat("{0}={1}&", key.ToLower(), queryParams[key]);
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length, 1);
            }
            else
            {
                return String.Empty;
            }

            return sb.ToString();
        }

        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="input">待加密字符串</param>
        ///   <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Encoding.UTF8.GetString(result);
        }

        #endregion

        public GroupCardPointResult GetPoint(GroupCardPointRequest request)
        {
            var body = OpenUrl(ConfigManager.GetGroupCardQueryScoreUrl(), "GET", null, null,
                               new Dictionary<string, string>
                                   {
                                       {"cardno", GetGroupCardEncryptString(request.CardNo)}
                                   }, null);

            var result = JsonExtension.FromJson<GroupCardPointResult>(body);

            return result;
        }

        public GroupCardInfoResult GetInfo(GroupCardInfoRequest request)
        {
            var body = OpenUrl(ConfigManager.GetGroupCardInfoUrl(), "GET", null, null,
                   new Dictionary<string, string>
                                   {
                                       {"cardno", GetGroupCardEncryptString(request.CardNo)},
                                       {"passwd",MD5Encrypt(request.Passwd)}
                                   }, null);

            var result = JsonExtension.FromJson<GroupCardInfoResult>(body);

            return result;
        }
    }
}
