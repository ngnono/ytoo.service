using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof(IRoleDataService))]
   public   class RoleDataService : IRoleDataService
    {
       public static List<Role> ListRole = new List<Role> { new Role() { RoleName = "1" }, new Role() {RoleName="hanyuxing" } };
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

        public ResultMsg Delete(OPCApp.Domain.Role model)
        {
            return ResultMsg.Success();
        }

        public OPCApp.Infrastructure.PageResult<OPCApp.Domain.Role> Search(IFilter filter)
        {
           return new OPCApp.Infrastructure.PageResult<Role>(ListRole,100);

        }
    }
}
