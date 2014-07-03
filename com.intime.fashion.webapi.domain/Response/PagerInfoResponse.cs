using System;
using System.Runtime.Serialization;
using Yintai.Architecture.Common.Models;

namespace com.intime.fashion.webapi.domain.Response
{
    public interface IPagerInfo
    {
        /// <summary>
        /// ҳ��(��1��ʼ)
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// ҳ��С
        /// </summary>
        int Size { get; }

        /// <summary>
        /// ��ҳ��
        /// </summary>
        int TotalPaged { get; }

        /// <summary>
        /// �ܼ�¼��
        /// </summary>
        int TotalCount { get; }
    }

    [DataContract]
    public abstract class PagerInfoResponse : BaseResponse, IPagerInfo
    {
        protected PagerInfoResponse(PagerRequest request)
            : this(request, 0)
        {
        }

        protected PagerInfoResponse(PagerRequest request, int totalCount)
        {
            this.Index = request.PageIndex;
            this.Size = request.PageSize;
            this.TotalCount = totalCount;
        }

        #region Implementation of IPagerInfoResponse

        /// <summary>
        /// ����
        /// </summary>
        [DataMember(Name = "pageindex", Order = 1)]
        public int Index { get; set; }

        /// <summary>
        /// ��С
        /// </summary>
        [DataMember(Name = "pagesize", Order = 2)]
        public int Size { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [DataMember(Name = "totalcount", Order = 3)]
        public int TotalCount { get; set; }

        /// <summary>
        /// ��ҳ��
        /// </summary>
        [DataMember(Name = "totalpaged", Order = 4)]
        public int TotalPaged
        {
            get
            {
                if (this.Size <= 0)
                    return 0;
                return (int)Math.Ceiling(this.TotalCount / (double)this.Size);
            }
            set { }
        }

        /// <summary>
        /// �Ƿ���ڷ�ҳ
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

        #endregion
    }
}