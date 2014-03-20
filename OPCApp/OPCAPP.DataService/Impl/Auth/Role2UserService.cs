using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using Intime.OPC.ApiClient;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
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


        public List<OPC_AuthUser> GetUserList(int roleId)
        {
            throw new NotImplementedException();
        }
    }
}
