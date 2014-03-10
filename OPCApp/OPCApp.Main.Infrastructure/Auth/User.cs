//===================================================================================
//
// 
//===================================================================================
// 作者：赵晓玉
// 创建日期：2014-2-5
//===================================================================================
// 修改记录
//
//===================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.Auth
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IUser
    {
        string UserID { get; }
        /// <summary>
        /// 用户名
        /// </summary>
        /// <value>The name.<alue>
        string Name { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        /// <value>The name of the pet.<alue>
        string ShowName { get; }

    }

    /// <summary>
    /// 角色接口
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        /// <value>The name.<alue>
        string Name { get; set; }
    }

}
