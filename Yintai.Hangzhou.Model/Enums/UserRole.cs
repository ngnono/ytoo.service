using System;

namespace Yintai.Hangzhou.Model.Enums
{
    /// <summary>
    /// 用户权限 Name 和数据库中的必须一致（属于字典表）
    /// </summary>
    [Flags]
    public enum UserRole
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 用户
        /// </summary>
        User = 1,

        /// <summary>
        /// 店长
        /// </summary>
        Manager = 2,

        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 4,

        /// <summary>
        /// 运营
        /// </summary>
        Operators = 8,

        /// <summary>
        /// 编辑
        /// </summary>
        Editor = 16,

        /// <summary>
        /// 来宾（匿名用户，未登录）
        /// </summary>
        Guest = 32
    }
}