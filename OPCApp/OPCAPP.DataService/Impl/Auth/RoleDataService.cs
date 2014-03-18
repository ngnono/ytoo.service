using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Infrastructure.DataService;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof(IRoleDataService))]
   public   class RoleDataService : IRoleDataService
    {
        public ResultMsg Add(OPC_AuthRole model)
        {
            return new ResultMsg() { IsSuccess=true,Msg="OK"};
        }

        public OPCApp.Infrastructure.DataService.ResultMsg Edit(OPC_AuthRole model)
        {
            return new ResultMsg() { IsSuccess = true, Msg = "OK" };
        }

        public ResultMsg Delete(OPC_AuthRole model)
        {
            return ResultMsg.Success();
        }

        public OPCApp.Infrastructure.PageResult<OPC_AuthRole> Search(IFilter filter)
        {
            return null;

        }
    }
}
