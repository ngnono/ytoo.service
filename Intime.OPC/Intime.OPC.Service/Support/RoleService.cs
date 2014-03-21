using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class RoleService : IRoleService
    {
        public RoleService(IRoleRepository roleRepository)
        {
            this._roleRepository = roleRepository;
        }

        private readonly IRoleRepository _roleRepository;


        public bool Create(OPC_AuthRole role)
        {
          return  _roleRepository.Create(role);
        }

        public bool Update(OPC_AuthRole role)
        {
            return _roleRepository.Update(role);
        }

        public bool Delete(int roleId)
        {
            return _roleRepository.Delete(roleId);
        }

        public System.Collections.Generic.IList<OPC_AuthRole> Select()
        {
            return _roleRepository.Select(t=>t.Name!="").ToList();
        }

        public bool IsStop(int roleId, bool bValid)
        {
            return _roleRepository.SetEnable(roleId, bValid);
        }


        public bool SetMenus(object roleMenuDto)
        {
            throw new System.NotImplementedException();
        }
    }
}