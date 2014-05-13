using System.Runtime.Serialization;

namespace OPCApp.Infrastructure.REST
{
    /// <summary>
    /// 数据响应
    /// </summary>
    /// <typeparam name="TData">响应数据类型</typeparam>
    [DataContract]
    public class Response<TData>
    {
        /// <summary>
        /// 返回状态
        /// </summary>
        [DataMember(Name = "status")]
        public bool Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// 返回状态码
        /// </summary>
        [DataMember(Name = "code")]
        public int Code { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        [DataMember(Name = "data")]
        public TData Data { get; set; }
    }
}
