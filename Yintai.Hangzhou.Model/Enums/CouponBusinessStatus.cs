namespace Yintai.Hangzhou.Model.Enums
{
    public enum CouponBusinessStatus
    {
        /// <summary>
        ///  
        /// </summary>
        Default = 0,

        /// <summary>
        /// 未使用 = 未过期
        /// </summary>
        Unused = 1,

        /// <summary>
        /// 使用的
        /// </summary>
        Use = 2,

        /// <summary>
        /// 正常 = 使用 + 未使用
        /// </summary>
        Normal = 3,

        /// <summary>
        /// 过期
        /// </summary>
        Expired = 4
    }
}