using System;
using System.Runtime.Serialization;

namespace Intime.OPC.MessageHandlers.AccessToken
{
    /// <summary>
    /// 用户访问令牌
    /// </summary>
    [DataContract]
    public class AccessTokenIdentity
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        [DataMember]
        public DateTime Expires { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public int UserId { get; set; }
    }
}
