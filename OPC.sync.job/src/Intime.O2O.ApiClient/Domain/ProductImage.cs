﻿
using System.Runtime.Serialization;

namespace Intime.O2O.ApiClient.Domain
{
    /// <summary>
    /// 商品图片
    /// </summary>
    [DataContract]
    public class ProductImage
    {
        /// <summary>
        /// 图片序号
        /// </summary>
        [DataMember(Name = "SEQNO")]
        public int SeqNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember(Name = "PRODUCTID")]
        public string ProductId { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [DataMember(Name = "URL")]
        public string Url { get; set; }

        /// <summary>
        /// 颜色Id
        /// </summary>
        [DataMember(Name = "COLORID")]
        public string ColorId { get; set; }
    }
}
