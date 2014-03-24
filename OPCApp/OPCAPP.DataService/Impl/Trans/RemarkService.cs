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
    [Export(typeof(IRemarkService))]
    public class RemarkService : IRemarkService
    {

        #region 销售单备注  保存、列表查询  接口实现
        /// <summary>
        /// 销售单备注保存
        /// </summary>
        /// <param name="saleComment"></param>
        /// <returns></returns>
        public bool WriteSaleRemark(OPC_SaleComment saleComment)
        {
            bool bFalg = RestClient.Post("sale/writesaleremark", saleComment);
            return bFalg;
        }
        /// <summary>
        /// 销售单备注查询
        /// </summary>
        /// <param name="saleIds"></param>
        /// <returns></returns>
        public PageResult<OPC_SaleComment> GetSaleRemark(string saleIds)
        {
            var lst = RestClient.Get<OPC_SaleComment>("sale/getsaleremarks", saleIds);
            return new PageResult<OPC_SaleComment>(lst, lst.Count);
        }

        #endregion

        #region 销售单备注  保存、列表查询  接口实现
        /// <summary>
        /// 销售单备注保存
        /// </summary>
        /// <param name="saleDetailsComment"></param>
        /// <returns></returns>
        public bool WriteSaleDetailsRemark(OPC_SaleDetailsComment saleDetailsComment)
        {
            bool bFalg = RestClient.Post("sale/writesaledetailsremark", saleDetailsComment);
            return bFalg;
        }
        /// <summary>
        /// 销售单备注查询
        /// </summary>
        /// <param name="SaleDetailIds"></param>
        /// <returns></returns>
        public PageResult<OPC_SaleDetailsComment> GetSaleDetailsRemark(string SaleDetailIds)
        {
            var lst = RestClient.Get<OPC_SaleDetailsComment>("sale/getsaledetailsremark", SaleDetailIds);
            return new PageResult<OPC_SaleDetailsComment>(lst, lst.Count);
        }

        #endregion
    }
}
