using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof(IRole2UserService))]
    public class Role2UserService : IRole2UserService
    {
        public ResultMsg SetUserByRole(int roleId, List<int> listUserId)
        {
            throw new NotImplementedException();
        }


        public List<OPC_AuthUser> GetUserListByRole(int roleId)
        {
            throw new NotImplementedException();
        }
    }
}
