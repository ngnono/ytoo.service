using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Main.Infrastructure.Auth
{

        public interface UserAuth
        {
            IUser CurrentUser { get; }
            /// <summary>
            /// 根据用户名密码登录
            /// </summary>
            /// <param name="userName">用户名</param>
            /// <param name="password">密码</param>
            /// <returns>验证成功，则返回空；失败时，返回错误提示</returns>
            string Login(string userName, string password);
            /// <summary>
            /// 注销登录
            /// </summary>
            void Logout();
            /// <summary>
            /// 验证用户是否是某个角色
            /// </summary>
            /// <param name="role">角色名称</param>
            /// <returns><c>true</c> 是, <c>false</c> 否</returns>
            bool CheckRole(string role);


        }
}
