using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

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

        public System.Collections.Generic.IList<OPC_AuthUser> Select()
        {
            return _accountRepository.Select(e=>true).ToList();
        }

        public bool IsStop(int userId, bool bValid)
        {
            return _accountRepository.SetEnable(userId, bValid);
        }
    }
}