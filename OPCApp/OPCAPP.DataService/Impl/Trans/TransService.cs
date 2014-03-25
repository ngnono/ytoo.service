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
                    url = "sale/GetSaleNoPickUp";
                    break;
                case EnumSearchSaleStatus.StoreInDataBaseSearchStatus:
                    url = "sale/GetSalePrintSale";
                    break;
                case EnumSearchSaleStatus.StoreOutDataBaseSearchStatus:
                    url = "sale/GetSaleShipInStorage";
                    break;
                case EnumSearchSaleStatus.PrintInvoiceSearchStatus:
                    url = "sale/GetSalePrintInvoice";
                    break;
                case EnumSearchSaleStatus.PrintExpressSearchStatus:
                    url = "sale/GetSalePrintExpress";
                    break;
            }
            var lst = RestClient.Get<OPC_Sale>(url, salesfilter);
            return new PageResult<OPC_Sale>(lst, lst.Count);
        }
        public bool PrintSale(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/PrintSale", saleOrderNoList);
        }
        /*完成打印销售单 状态*/ //SetSaleOrderPrintSale
        public bool SetStatusAffirmPrintSaleFinish(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/SetSaleOrderFinishPrintSale", saleOrderNoList);
        }
        /*设置销售单入库 状态*/
        public bool SetStatusStoreInSure(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/SetSaleOrderShipInStorage", saleOrderNoList);
        }
        /*设置销售单出库 状态*/
        public bool SetSaleOrderStockOut(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/SetSaleOrderStockOut", saleOrderNoList);
        }
        /*设置销售单缺货 状态*/
        public bool SetStatusSoldOut(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/SetSaleOrderStockOut", saleOrderNoList);
        }


        public PageResult<OPC_SaleDetail> SelectSaleDetail(string saleOrderNo)
        {
            var lst = RestClient.Get<OPC_SaleDetail>("sale/selectSale",string.Format("saleOrderNo={0}",saleOrderNo));
            return new PageResult<OPC_SaleDetail>(lst, 100);
        }



        public bool SetStatusPrintInvoice(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/SetSaleOrderPrintInvoice", saleOrderNoList);
        }


        public bool SetStatusPrintExpress(IList<string> saleOrderNoList)
        {
            return RestClient.Put("sale/SetSaleOrderPrintExpress", saleOrderNoList);
        }
    }
}
