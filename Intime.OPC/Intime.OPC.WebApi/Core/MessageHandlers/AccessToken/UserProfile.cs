using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intime.OPC.WebApi.Core.MessageHandlers.AccessToken
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [DataContract]
    public class UserProfile
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 专柜Id
        /// </summary>
        [DataMember]
        public int? SectionId { get; set; }

        /// <summary>
        /// 组织Id
        /// </summary>
        [DataMember]
        public string OrganizationId { get; set; }

        /// <summary>
        /// 数据验证Id
        /// </summary>
        [DataMember]
        public string DataAuthId { get; set; }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        [DataMember]
        public IEnumerable<string> Roles { get; set; }
    }
}
