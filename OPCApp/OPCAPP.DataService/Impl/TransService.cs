using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using Intime.OPC.ApiClient;
using Intime.OPC.Domain.Models;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof(ITransService))]
    public class TransService : ITransService
    {

        public bool Finish(Dictionary<string,string> sale)
        {
            bool bFalg = RestClient.Put("trans/finish", sale);
            return bFalg;
        }

        public PageResult<OPC_Sale> Search(IDictionary<string, object> filter)
        {
            var lst = RestClient.Get<OPC_Sale>("trans/selectsales", filter);
            return new PageResult<OPC_Sale>(lst, lst.Count);
        }

        public PageResult<OPC_SaleDetail> SelectSaleDetail(IDictionary<string, object> filter)
        {
            var lst = RestClient.Get<OPC_SaleDetail>("trans/SelectSaleDetail", filter);
            return new PageResult<OPC_SaleDetail>(lst, lst.Count);
        }
        

        /*
        public Infrastructure.DataService.ResultMsg Finish(Domain.Models.OPC_AuthUser user)
        {
            var bFalg = RestClient.Put("/api/account/updateuser", user);
            return new ResultMsg() { IsSuccess = bFalg, Msg = "保存成功" };
        }

        public Infrastructure.DataService.ResultMsg Delete(Domain.Models.OPC_AuthUser user)
        {
            var bFalg = RestClient.Put("/api/account/deleteuser", user);
            return new ResultMsg() { IsSuccess = bFalg, Msg = "删除错误" };
        }

        public PageResult<OPC_AuthUser> Search(Infrastructure.DataService.IFilter filter)
        {
            var lst= RestClient.Get<OPC_AuthUser>("/api/account/selectuser", filter.GetFilter());
            return new PageResult<OPC_AuthUser>(lst,lst.Count);
        }
        */

    
    }
}
