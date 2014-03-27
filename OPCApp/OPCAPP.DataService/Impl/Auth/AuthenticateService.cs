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

        public bool SetIsStop(int userId, bool isStop)
        {
            return false;
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
                bool bFalg = RestClient.Put("account/addUser", user);
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

        public ResultMsg Delete(OPC_AuthUser user)
        {
            try
            {
                var oo = new {userId = user.Id};
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
                IList<OPC_AuthUser> lst = RestClient.Get<OPC_AuthUser>("account/selectuser", strParmas.TrimEnd('&'));
                return new PageResult<OPC_AuthUser>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }
    }
}