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
using Intime.OPC.Service.Security;

namespace Intime.OPC.Service.Support
{
    public class AccountService :BaseService<OPC_AuthUser>, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrgInfoRepository _orgInfoRepository;
        private IRoleUserRepository _roleUserRepository;
        private ISectionRepository _sectionRepository;

        public AccountService(IAccountRepository accountRepository,IOrgInfoRepository orgInfoRepository, IRoleUserRepository roleUserRepository, ISectionRepository sectionRepository):base(accountRepository)
        {
            _accountRepository = accountRepository;
            _orgInfoRepository = orgInfoRepository;
            _roleUserRepository = roleUserRepository;
            _sectionRepository = sectionRepository;
        }

        public override bool DeleteById(int id)
        {
            _roleUserRepository.DeleteByUserID(id);
            return base.DeleteById(id);
        }

        #region IAccountService Members

        public bool Add(OPC_AuthUser t)
        {
            t.Password = t.Password.MD5CSP();
            return  base.Add(t);
        }

        public bool Update(OPC_AuthUser t)
        {
            var u = _accountRepository.GetByID(t.Id);
            if (u==null)
            {
                throw new UserNotExistException(t.Id);
            }
            t.Password = u.Password;
           return   base.Update(t);
        }

        public AuthUserDto Get(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("用户或密码为空");
            }

            var pwd=  password.MD5CSP();
            var user = _accountRepository.Get(userName, pwd);
            if (user==null)
            {
                throw new Exception("用户名或密码不正确");
            }
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

            dto.SectionID= _sectionRepository.GetByStoreIDs(dto.StoreIDs).Select<Section,int>(t=>t.Id).Distinct().ToList();
        
            //dto.SectionIDs = _orgInfoRepository.GetByOrgType(user.DataAuthId, EnumOrgType.Section.AsID()).Select(t => t.StoreOrSectionID.Value).Distinct().ToList();

            return dto;
        }

       
        public void ResetPassword(int userId)
        {
             var u= _accountRepository.GetByID(userId);
            if (u!=null && !u.IsSystem && u.IsValid==true)
            {
                u.Password = "123456".MD5CSP();
                _accountRepository.Update(u);
            }
        }

        public void ChangePassword(int userid, string oldpassword, string newpassword)
        {
            var u = _accountRepository.GetByID(userid);
            if (u == null)
            {
                throw new Exception("用户不存在");
            }
            if (u.Password != oldpassword)
            {
                throw new Exception("密码不正确");
            }
            u.Password = newpassword.MD5CSP();
            _accountRepository.Update(u);
        }

        #endregion

        protected PageResult<AuthUserDto> OpcResult2Result(PageResult<OPC_AuthUser> result)
        {
             var lstOrg = _orgInfoRepository.GetAll(1, 10000);
            IList<AuthUserDto> lstUserDtos=new List<AuthUserDto>();

            foreach (var user in result.Result)
            {
                var u = AutoMapper.Mapper.Map<OPC_AuthUser, AuthUserDto>(user);
                var org = lstOrg.Result.FirstOrDefault(t => t.OrgID == user.DataAuthId);
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

