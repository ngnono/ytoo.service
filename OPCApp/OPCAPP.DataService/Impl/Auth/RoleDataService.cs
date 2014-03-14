using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Infrastructure.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.DataService.Impl
{
   public   class RoleDataService : IRoleDataService
    {
       public static List<Role> ListRole = new List<Role>() { new Role() { RoleName = "1" }, new Role() {RoleName="hanyuxing" } };
        public ResultMsg Add(OPCApp.Domain.Role model)
        {
            ListRole.Add(model);
            return new ResultMsg() { IsSuccess=true,Msg="OK"};
        }

        public OPCApp.Infrastructure.DataService.ResultMsg Edit(OPCApp.Domain.Role model)
        {
            ListRole.Add(model);
            return new ResultMsg() { IsSuccess = true, Msg = "OK" };
        }

        public OPCApp.Infrastructure.DataService.ResultMsg Delete(OPCApp.Domain.Role model)
        {
            throw new NotImplementedException();
        }

        public OPCApp.Infrastructure.PageResult<OPCApp.Domain.Role> Search(IFilter filter)
        {
           return new OPCApp.Infrastructure.PageResult<Role>(ListRole,100);

        }
    }
}
