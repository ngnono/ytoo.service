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
    [Export(typeof(IRole2MenuService))]
    public class Role2MenuService : IRole2MenuService
    {



        public ResultMsg SetMenuByRole(int roleId, List<int> listMenuId)
        {
            throw new NotImplementedException();
        }


        public List<OPC_AuthMenu> GetMenuList(int roleId)
        {
            throw new NotImplementedException();
        }
    }
}
