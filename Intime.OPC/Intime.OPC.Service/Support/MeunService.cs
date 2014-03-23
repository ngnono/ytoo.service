using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class MenuService :IMenuService
    {
        private readonly IMenuRepository _menuRepository;


        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
       
        public IEnumerable<OPC_AuthMenu> SelectByRoleID(int roleID)
        {
           
            return _menuRepository.GetMenusByRoleID(roleID);
        }



        public IEnumerable<OPC_AuthMenu> SelectByUserID(int userID)
        {
            return _menuRepository.GetMenusByUserID(userID);
        }
    }
}