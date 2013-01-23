using System;

namespace Yintai.Hangzhou.Model.Enums
{
    /// <summary>
    /// 用户的等级
    /// </summary>
    [Flags]
    public enum UserLevel
    {
        /// <summary>
        /// 啥都不是
        /// </summary>
        None = 0,

        /// <summary>
        /// 普通用户
        /// </summary>
        User = 1,

        /// <summary>
        /// 达人
        /// </summary>
        Daren = 2,

        ///// <summary>
        ///// 店长
        ///// </summary>
        //Manager = 4,
    }
}
