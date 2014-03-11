namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public interface IPagerInfo
    {
        /// <summary>
        /// 页码(从1起始)
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        int Size { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPaged { get; }

        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount { get; }

    }
}