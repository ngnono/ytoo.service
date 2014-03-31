using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class AccountService :BaseService, IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        #region IAccountService Members

        public OPC_AuthUser Get(string userName, string password)
        {
            return _accountRepository.Get(userName, password);
        }

        public bool Create(OPC_AuthUser user)
        {
            return _accountRepository.Create(user);
        }

        public bool Update(OPC_AuthUser user)
        {
            return _accountRepository.Update(user);
        }

        public bool Delete(int userId)
        {
            return _accountRepository.Delete(userId);
        }

        public PageResult<OPC_AuthUser> Select(int pageIndex, int pageSize = 20)
        {
            return _accountRepository.All(pageIndex, pageSize);
        }

        public bool IsStop(int userId, bool bValid)
        {
            return _accountRepository.SetEnable(userId, bValid);
        }

        public PageResult<OPC_AuthUser> GetUsersByRoleID(int roleId, int pageIndex, int pageSize = 20)
        {
            return GetUsersByRoleID(roleId,pageIndex,pageSize);
        }

        #endregion
    }
}