using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Intime.OPC.Domain.Models;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof(ITransService))]
    public class TransService : ITransService
    {


        public PageResult<OPC_Sale> Search(string salesfilter,EnumSearchSaleStatus enumSearchSaleStatus)
        {
            string url = "";
            switch (enumSearchSaleStatus)
            {
                case EnumSearchSaleStatus.CompletePrintSearchStatus:
                    url = "sale/selectSale";
                    break;
                case EnumSearchSaleStatus.StoreInDataBaseSearchStatus:
                    url = "";
                    break;
                default:
                    break;
            }
            var lst = RestClient.Get<OPC_Sale>("sale/selectSale", salesfilter);
            return new PageResult<OPC_Sale>(lst, lst.Count);
        }
        public bool PrintSale(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/PrintSale", saleOrderNoList);
        }
        /*完成打印销售单 状态*/
        public bool SetStatusAffirmPrintSaleFinish(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/SetNoPickUp", saleOrderNoList);
        }
        /*设置销售单入库 状态*/
        public bool SetStatusStoreInSure(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/selectSale", saleOrderNoList);
        }
        /*设置销售单出库 状态*/
        public bool SetStatusSoldOut(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/selectSale", saleOrderNoList);
        }


        public PageResult<OPC_SaleDetail> SelectSaleDetail(string saleOrderNo)
        {
            var lst = RestClient.Get<OPC_SaleDetail>("sale/selectSale",string.Format("saleOrderNo={0}",saleOrderNo));
            return new PageResult<OPC_SaleDetail>(lst, 100);
        }

    }
}
