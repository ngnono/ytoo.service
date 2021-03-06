﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Intime.OPC.MessageHandlers.AccessToken;
using Intime.OPC.WebApi.Core.Security.Cryptography;

namespace Intime.OPC.WebApi.Core.Security
{
    public static class SecurityUtils
    {
        #region fields

        private const string Accesstoken = "AccessToken";
        private const string Vector = "abcdefghijklmnox";
        private const string Hash = "SHA1";
        private const int KeySize = 256;

        #endregion

        public static string CreateAccessToken(int userId, DateTime expires)
        {
            var token = new AccessTokenIdentity
            {
                UserId = userId,
                Expires = expires
            };

            string input = ToJson(token);

            return AesEncrypt(input);
        }

        public static AccessTokenIdentity GetAccessToken(string input)
        {
            string temp = AesDecrypt(input);

            if (string.IsNullOrWhiteSpace(temp))
            {
                return null;
            }
            var token = Deserialize<AccessTokenIdentity>(temp);

            return token;
        }

        #region Helper

        private static string AesEncrypt(string input)
        {
            return CryptoHelper.Base64AESEncrypt(input,
                Accesstoken,
                Accesstoken,
                Hash,
                1,
                Vector,
                KeySize
                );
        }

        private static string AesDecrypt(string input)
        {
            return CryptoHelper.Base64AESDecrypt(input,
                Accesstoken,
                Accesstoken,
                Hash,
                1,
                Vector,
                KeySize
                );
        }

        private static string ToJson<T>(T target)
        {
            var dcs = new DataContractJsonSerializer(target.GetType());
            string result;

            using (var stream = new MemoryStream())
            {
                dcs.WriteObject(stream, target);
                result = Encoding.UTF8.GetString(stream.ToArray());
            }

            return result;
        }

        private static T Deserialize<T>(string json) where T : class
        {
            var ds = new DataContractJsonSerializer(typeof (T));

            T obj;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T) ds.ReadObject(ms);
                ms.Close();
            }

            return obj;
        }

        #endregion
    }
}