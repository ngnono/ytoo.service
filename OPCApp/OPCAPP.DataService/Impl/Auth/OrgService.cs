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
    [Export(typeof(IOrgService))]
    public class OrgService : IOrgService
    {
        private string _nameP;
        public string Name {
            get { return this._nameP; } 
            set { _nameP = value; }
        }

        public ResultMsg Add(OPC_OrgInfo org)
        {
            try
            {
                if (org == null)
                {
                    return new ResultMsg {IsSuccess = false, Msg = "增加错误"};
                }

                var ent = RestClient.PutReturnModel("org/addOrg", org);
                return new ResultMsg {IsSuccess = ent==null, Msg = "保存成功",Data = ent};
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
                bool bFalg = RestClient.Put("org/updateOrg", org);
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
                bool bFalg = RestClient.Put("org/deleteorg", org.Id);
                return new ResultMsg {IsSuccess = bFalg, Msg = "删除错误"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }


        public IList<OPC_OrgInfo> Search()
        {
            try
            {
                var lst = RestClient.Get<OPC_OrgInfo>("org/getall","");
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        PageResult<OPC_OrgInfo> IBaseDataService<OPC_OrgInfo>.Search(IDictionary<string, object> iDicFilter)
        {
            throw new NotImplementedException();
        }
    }
}