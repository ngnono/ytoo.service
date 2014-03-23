using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            this._roleRepository = roleRepository;
         
        }

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

        public IList<OPC_AuthRole> Select()
        {
            return _roleRepository.All().ToList();
        }

        public bool IsStop(int roleId, bool bValid)
        {
            return _roleRepository.SetEnable(roleId, bValid);
        }


        public bool SetMenus(object roleMenuDto)
        {
            throw new System.NotImplementedException();
        }


        public IEnumerable<OPC_AuthRole> GetByUserID(int userID)
        {
            return _roleRepository.GetByUserID(userID);
        }
    }
}