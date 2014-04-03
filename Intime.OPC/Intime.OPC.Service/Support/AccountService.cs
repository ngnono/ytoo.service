using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class AccountService :BaseService<OPC_AuthUser>, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrgInfoRepository _orgInfoRepository;

        public AccountService(IAccountRepository accountRepository,IOrgInfoRepository orgInfoRepository):base(accountRepository)
        {
            _accountRepository = accountRepository;
            _orgInfoRepository = orgInfoRepository;
        }

        #region IAccountService Members

        public OPC_AuthUser Get(string userName, string password)
        {
            return _accountRepository.Get(userName, password);
        }

        public PageResult<OPC_AuthUser> Select(string orgid, string name, int pageIndex, int pageSize = 20)
        {
            return _accountRepository.GetByOrgId(orgid, name, pageIndex, pageSize);
        }

        public PageResult<OPC_AuthUser> SelectByLogName(string orgid, string loginName, int pageIndex, int pageSize = 20)
        {
            return _accountRepository.GetByLoginName(orgid, loginName, pageIndex, pageSize);
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

        public UserDto GetByUserID(int userID)
        {
            var user = _accountRepository.GetByID(userID);
            if (user==null)
            {
                throw new UserNotExistException(userID);
            }
            if (!user.IsValid.HasValue ||!user.IsValid.Value)
            {
                throw new  UserNotValidException(userID);
            }
            UserDto dto=new UserDto();
            dto.UserID = userID;

            dto.StoreIDs = _orgInfoRepository.GetByOrgType(user.DataAuthId, EnumOrgType.Store.AsID()).Select(t=>t.StoreOrSectionID.Value).Distinct().ToList();
            dto.SectionIDs = _orgInfoRepository.GetByOrgType(user.DataAuthId, EnumOrgType.Section.AsID()).Select(t => t.StoreOrSectionID.Value).Distinct().ToList();

            return dto;
        }

        public PageResult<OPC_AuthUser> GetUsersByOrgId(int orgId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

