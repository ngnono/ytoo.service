using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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

        public AuthUserDto Get(string userName, string password)
        {
            
            var user= _accountRepository.Get(userName, password);
            var dto= AutoMapper.Mapper.Map<OPC_AuthUser,AuthUserDto>(user);
            var org = _orgInfoRepository.GetByOrgID(user.DataAuthId);
            if (org!=null)
            {
                dto.DataAuthName =org.OrgName;
            }
            
            return dto;
        }

        public PageResult<AuthUserDto> Select(string orgid, string name, int pageIndex, int pageSize = 20)
        {
            var lst= _accountRepository.GetByOrgId(orgid, name, pageIndex, pageSize);
            return OpcResult2Result(lst);
        }

        public PageResult<AuthUserDto> SelectByLogName(string orgid, string loginName, int pageIndex, int pageSize = 20)
        {
            var lst= _accountRepository.GetByLoginName(orgid, loginName, pageIndex, pageSize);
            return OpcResult2Result(lst);
        }

        public PageResult<AuthUserDto> Select(int pageIndex, int pageSize = 20)
        {
            var lst = _accountRepository.All(pageIndex, pageSize);
            return OpcResult2Result(lst);
        }

        public bool IsStop(int userId, bool bValid)
        {
            return _accountRepository.SetEnable(userId, bValid);
        }

        public PageResult<AuthUserDto> GetUsersByRoleID(int roleId, int pageIndex, int pageSize = 20)
        {
            var lst = _accountRepository.GetByRoleId(roleId, pageIndex, pageSize);
            return OpcResult2Result(lst);
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

     

        #endregion

        protected PageResult<AuthUserDto> OpcResult2Result(PageResult<OPC_AuthUser> result)
        {
             var lstOrg = _orgInfoRepository.GetAll(1, 10000);
            IList<AuthUserDto> lstUserDtos=new List<AuthUserDto>();

            foreach (var user in result.Result)
            {
                var u = AutoMapper.Mapper.Map<OPC_AuthUser, AuthUserDto>(user);
                var org = lstUserDtos.FirstOrDefault(t => t.OrgId == user.DataAuthId);
                if (org!=null)
                {
                    u.DataAuthName = org.OrgName;
                }
                lstUserDtos.Add(u);
            }

            return new PageResult<AuthUserDto>(lstUserDtos,result.TotalCount);
        }
    }
}

