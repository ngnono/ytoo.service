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
    [Export(typeof (IOrderService))]
    public class OrgService : IOrderService
    {

        public ResultMsg Add(OPC_OrgInfo org)
        {
            try
            {
                if (org == null)
                {
                    return new ResultMsg {IsSuccess = false, Msg = "增加错误"};
                }

                bool bFalg = RestClient.Put("account/addUser", org);
                return new ResultMsg {IsSuccess = bFalg, Msg = "保存成功"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }

        public ResultMsg Edit(OPC_OrgInfo org)
        {
            try
            {
                bool bFalg = RestClient.Put("account/updateuser", org);
                return new ResultMsg {IsSuccess = bFalg, Msg = "保存成功"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }

        public ResultMsg Delete(OPC_OrgInfo org)
        {
            try
            {
                var oo = new { userId = org.Id };
                bool bFalg = RestClient.Put("account/deleteuser", org.Id);
                return new ResultMsg {IsSuccess = bFalg, Msg = "删除错误"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }


        public PageResult<OPC_OrgInfo> Search(IDictionary<string, object> iDicFilter)
        {
            try
            {
                string strParmas = iDicFilter.Keys.Aggregate("",
                    (current, key) => current + string.Format("{0}={1}&", key, iDicFilter[key]));
                PageResult<OPC_OrgInfo> lst = RestClient.GetPage<OPC_OrgInfo>("account/selectuser", strParmas.Trim('&'));
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}