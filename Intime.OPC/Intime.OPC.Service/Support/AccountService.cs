using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            this._accountRepository = accountRepository;
        }

        public OPC_AuthUser Get(string userName, string password)
        {
            return _accountRepository.Get(userName, password);
        }
    }
}
