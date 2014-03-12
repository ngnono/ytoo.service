using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

using System.Linq;

namespace Intime.OPC.Repository.Support
{
    public class AccountRepository : IAccountRepository
    {
        public OPC_AuthUser Get(string userName, string password)
        {
            using (var db = new YintaiHZhouContext())
            {
                var query = from b in db.OPC_AuthUser
                            where (b.LogonName == userName && b.Password == password)
                            select b;

                return query.FirstOrDefault();
            }
        }
    }
}
