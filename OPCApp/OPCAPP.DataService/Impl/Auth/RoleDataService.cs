using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof(IRoleDataService))]
   public   class RoleDataService : IRoleDataService
    {
        private string url = "Role/AddRole";
        public ResultMsg Add(OPC_AuthRole model)
        {
            var result = RestClient.Post<OPC_AuthRole>("Role/AddRole", model);
            if (result)
            {
               return  ResultMsg.Success();
            }
            return ResultMsg.Failure("添加失败！");
        }

        public ResultMsg Edit(OPC_AuthRole model)
        {
            var result = RestClient.Post<OPC_AuthRole>("Role/UpdateRole", model);
            if (result)
            {
                return ResultMsg.Success();
            }
            return ResultMsg.Failure("更新失败！");
        }

        public ResultMsg Delete(OPC_AuthRole model)
        {
            var result = RestClient.Delete<OPC_AuthRole>("Role/DeleteRole/"+model.Id);
            if (result)
            {
                return ResultMsg.Success();
            }
            return ResultMsg.Failure("删除失败！");
        }

        public PageResult<OPC_AuthRole> Search(IFilter filter)
        {

            var result = RestClient.Get<OPC_AuthRole>("Role/SelectRole",null);
            return new PageResult<OPC_AuthRole>(result,result.Count);
        }
    }
}
