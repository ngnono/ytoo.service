using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using Intime.OPC.ApiClient;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof(IAuthenticateService))]
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

        public Infrastructure.DataService.ResultMsg Add(Domain.Models.OPC_AuthUser user)
        {
            if (user == null)
            {
                return new ResultMsg() { IsSuccess = false, Msg = "增加错误" };
            }
            user.IsValid = true;
            user.CreateDate = DateTime.Now;
            user.CreateUserId = 1;
            user.UpdateDate = DateTime.Now;
            user.UpdateUserId = 1;
            var bFalg= RestClient.Put("account/addUser", user);
            return  new ResultMsg(){IsSuccess = bFalg,Msg = "保存成功"};
        }

        public Infrastructure.DataService.ResultMsg Edit(Domain.Models.OPC_AuthUser user)
        {
            var bFalg = RestClient.Put("account/updateuser", user);
            return new ResultMsg() { IsSuccess = bFalg, Msg = "保存成功" };
        }

        public Infrastructure.DataService.ResultMsg Delete(Domain.Models.OPC_AuthUser user)
        {
            var oo = new { userId =user.Id};
            var bFalg = RestClient.Put("account/deleteuser",user.Id);
            return new ResultMsg() { IsSuccess = bFalg, Msg = "删除错误" };
        }


        public PageResult<OPC_AuthUser> Search(IDictionary<string, object> iDicFilter)
        {
            string strParmas = iDicFilter.Keys.Aggregate("", (current, key) => current + string.Format("{0}={1},", key, iDicFilter[key]));
            var lst = RestClient.Get<OPC_AuthUser>("account/selectuser", strParmas.TrimEnd(','));
            return new PageResult<OPC_AuthUser>(lst, lst.Count);
        }
    }
}
