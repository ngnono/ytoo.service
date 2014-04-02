using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IAccountService : IService,ICanAdd<OPC_AuthUser>,ICanDelete,ICanUpdate<OPC_AuthUser>
    {
        OPC_AuthUser Get(string userName, string password);
  
        PageResult<OPC_AuthUser> Select(string orgid, string name,  int pageIndex, int pageSize = 20);
        PageResult<OPC_AuthUser> SelectByLogName(string orgid, string loginName, int pageIndex, int pageSize = 20);
        bool IsStop(int userId, bool bValid);

        PageResult<OPC_AuthUser> GetUsersByRoleID(int roleId, int pageIndex, int pageSize = 20);

        UserDto GetByUserID(int userID);

       
    }
}