using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Routing;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    [DataContract]
    public abstract class PagerInfo : IPagerInfo
    {
        protected PagerInfo(PagerRequest request)
            : this(request, 0)
        {
        }

        protected PagerInfo(PagerRequest request, int totalCount)
        {
            this.Index = request.PageIndex;
            this.Size = request.PageSize;
            this.TotalCount = totalCount;
        }

        #region Implementation of IPagerInfoResponse

        /// <summary>
        /// 索引
        /// </summary>
        [DataMember(Name = "pageindex", Order = 1)]
        public int Index { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        [DataMember(Name = "pagesize", Order = 2)]
        public int Size { get; set; }

        /// <summary>
        /// 总数据量
        /// </summary>
        [DataMember(Name = "totalcount", Order = 3)]
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [DataMember(Name = "totalpaged", Order = 4)]
        public int TotalPaged
        {
            get
            {
                return (int)Math.Ceiling(this.TotalCount / (double)this.Size);
            }
            set { }
        }

        /// <summary>
        /// 是否存在分页
        /// </summary>
        [DataMember(Name = "ispaged", Order = 5)]
        public bool IsPaged
        {
            get
            {
                return Size < TotalCount;
            }
            set { }
        }

        /// <summary>
        /// query
        /// </summary>
        [IgnoreDataMember]
        public string Query
        {
            get { return HttpContext.Current.Request.Url.Query; }
            set { }
        }

        /// <summary>
        /// query
        /// </summary>
        [IgnoreDataMember]
        public NameValueCollection QueryCollection
        {
            get { return HttpContext.Current.Request.QueryString; }
        }

        /// <summary>
        /// Url
        /// </summary>
        [IgnoreDataMember]
        public string UL { get; set; }


        #endregion
    }
    public class Pager<T> :PagerInfo
    {
        public Pager(PagerRequest request): base(request)
        {
        }

        public Pager(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }
        [DataMember]
        public IEnumerable<T> Data { get; set; }
    }
}