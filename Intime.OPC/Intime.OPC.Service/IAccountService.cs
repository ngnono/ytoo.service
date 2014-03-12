using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IAccountService
    {
        OPC_AuthUser Get(string userName, string password);
    }
}
