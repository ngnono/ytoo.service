using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class MenuService :IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository accountMenutory)
        {
            _menuRepository = accountMenutory;
        }
        public IEnumerable<OPC_AuthMenu> Select()
        {
            return  _menuRepository.Select(e => true).ToList();
        }
    }
}