using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IAccountRepository
    {
        OPC_AuthUser Get(string userName, string password);
    }
}
