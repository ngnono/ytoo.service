using System.Collections.Generic;
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
           // return null;
            //var key = dictionary.Keys.FirstOrDefault();
            //var v = dictionary.Values.FirstOrDefault();
            return _accountRepository.Select(t => t.IsValid.HasValue==true).ToList();
            //switch (key)
            //{
            //    case "0":
            //        query = _accountRepository.Select(t => t.LogonName == v);
            //        return query.ToList();
            //    case "1":
            //        query = _accountRepository.Select(t => t. == v);
            //        return query.ToList();
            //    case "2":
            //        query = _accountRepository.Select(t => t.Name == v);
            //        return query.ToList();
            //    case "3":
            //        query = _accountRepository.Select(t => t.SectionId == v);
            //        return query.ToList();
            //    case "4":
            //        query = _accountRepository.Select(t => t.OrgId == v);
            //        return query.ToList();

            //}
            //return query.ToList();
        }

        public bool IsStop(int userId, bool bValid)
        {
            return _accountRepository.SetEnable(userId, bValid);
        }


       
    }
}