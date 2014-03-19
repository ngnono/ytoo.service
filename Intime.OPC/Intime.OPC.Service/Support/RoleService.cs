using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class RoleService : IRoleService
    {
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
            return _roleRepository.Select();
        }

        public bool IsStop(int roleId, bool bValid)
        {
            return _roleRepository.IsStop(roleId, bValid);
        }
    }
}