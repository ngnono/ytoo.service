using Intime.OPC.MessageHandlers.AccessToken;
using Intime.OPC.WebApi.Core.Security.Cryptography;

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Intime.OPC.WebApi.Core.Security
{
    public class SecurityUtils
    {
        #region fields

        private static readonly string VECTOR = "abcdefghijklmnox";
        private static readonly string HASH = "SHA1";
        private static readonly int KEY_SIZE = 256;
        private const string ACCESSTOKEN = "AccessToken";

        #endregion

        public static string CreateAccessToken(int userId, DateTime expires)
        {
            var token = new AccessTokenIdentity()
            {
                UserId = userId,
                Expires = expires
            };

            var input = ToJson<AccessTokenIdentity>(token);

            return AESEncrypt(input);
        }

        public static AccessTokenIdentity GetAccessToken(string input)
        {
            var temp = AESDecrypt(input);

            if (string.IsNullOrWhiteSpace(temp))
            {
                return null;
            }
            var token = Deserialize<AccessTokenIdentity>(temp);

            return token;
        }

        #region Helper

        private static string AESEncrypt(string input)
        {
            return CryptoHelper.Base64AESEncrypt(input,
               ACCESSTOKEN,
               ACCESSTOKEN,
                HASH,
                1,
                VECTOR,
                KEY_SIZE
            );
        }

        private static string AESDecrypt(string input)
        {
            return CryptoHelper.Base64AESDecrypt(input,
                ACCESSTOKEN,
                ACCESSTOKEN,
                HASH,
                1,
                VECTOR,
                KEY_SIZE
            );
        }

        public static string ToJson<T>(T target)
        {
            var dcs = new DataContractJsonSerializer(target.GetType());
            var result = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                dcs.WriteObject(stream, target);
                result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }

            return result;
        }

        public static T Deserialize<T>(string json) where T : class
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));

            T obj = null;

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T)ds.ReadObject(ms);
                ms.Close();
            }

            return obj;
        }

        #endregion
    }
}
