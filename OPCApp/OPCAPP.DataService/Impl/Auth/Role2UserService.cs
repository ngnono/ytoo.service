using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof (IRole2UserService))]
    public class Role2UserService : IRole2UserService
    {
        public ResultMsg SetUserByRole(int roleId, List<int> listUserId)
        {
            try
            {
                bool bFalg = RestClient.Post("role/setUsers", new {roleId, listMenuId = listUserId});
                return new ResultMsg {IsSuccess = true, Msg = "保存成功"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "API发送失败"};
            }
        }


        public List<OPC_AuthUser> GetUserListByRole(int roleId)
        {
            try
            {
                return
                    RestClient.Get<OPC_AuthUser>("account/GetUsersByRoleID", string.Format("roleId={0}", roleId))
                        .ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}