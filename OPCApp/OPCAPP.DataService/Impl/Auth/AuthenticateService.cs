using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof (IAuthenticateService))]
    public class AuthenticateService : IAuthenticateService
    {
        public string Login(string userName, string password)
        {
            return "OK";
        }

        public bool SetIsStop(OPC_AuthUser user)
        {
            try
            {
                bool bFalg = RestClient.Put("account/Enable", user);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ResultMsg Add(OPC_AuthUser user)
        {
            try
            {
                if (user == null)
                {
                    return new ResultMsg {IsSuccess = false, Msg = "增加错误"};
                }
                user.IsValid = true;
                user.CreateDate = DateTime.Now;
                user.CreateUserId = 1;
                user.UpdateDate = DateTime.Now;
                user.UpdateUserId = 1;
                bool bFalg = RestClient.Post("account/addUser", user);
                return new ResultMsg {IsSuccess = bFalg, Msg = "保存成功"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }

        public ResultMsg Edit(OPC_AuthUser user)
        {
            try
            {
                bool bFalg = RestClient.Put("account/updateuser", user);
                return new ResultMsg {IsSuccess = bFalg, Msg = "保存成功"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }

        public ResultMsg ResetPassword(OPC_AuthUser user)
        {
            try
            {
                bool bFalg = RestClient.Put("account/ResetPassword", user.Id);
                return new ResultMsg {IsSuccess = bFalg, Msg = "删除错误"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }

        public ResultMsg Delete(OPC_AuthUser user)
        {
            try
            {
                bool bFalg = RestClient.Put("account/deleteuser", user.Id);
                return new ResultMsg {IsSuccess = bFalg, Msg = "删除错误"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }


        public PageResult<OPC_AuthUser> Search(IDictionary<string, object> iDicFilter)
        {
            try

            {
                string strParmas = iDicFilter.Keys.Aggregate("",
                    (current, key) => current + string.Format("{0}={1}&", key, iDicFilter[key]));
                PageResult<OPC_AuthUser> lst = RestClient.GetPage<OPC_AuthUser>("account/selectuser",
                    strParmas.Trim('&'));
                return lst;
            }
            catch (Exception ex)
            {
                return new PageResult<OPC_AuthUser>(new List<OPC_AuthUser>(), 0);
            }
        }
    }
}