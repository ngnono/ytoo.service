using System.Collections.Generic;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface
{
    public interface IRole2MenuService
    {
        ResultMsg SetMenuByRole(int roleId, List<int> listMenuId);
        List<OPC_AuthMenu> GetMenuList(int roleId);
    }
}