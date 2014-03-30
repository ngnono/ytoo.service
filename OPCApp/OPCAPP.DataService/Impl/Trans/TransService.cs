using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Dto;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (ITransService))]
    public class TransService : ITransService
    {
        public PageResult<OPC_Sale> Search(string salesfilter, EnumSearchSaleStatus enumSearchSaleStatus)
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
                case EnumSearchSaleStatus.ShipedSearchStatus:
                    url = "sale/GetShipped";
                    break;
            }
            try
            {
                IList<OPC_Sale> lst = RestClient.Get<OPC_Sale>(url, salesfilter);
                return new PageResult<OPC_Sale>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public PageResult<OPC_ShippingSale> GetListShip(string filter)
        {
            try
            {
                IList<OPC_ShippingSale> shipSales = RestClient.Get<OPC_ShippingSale>("trans/GetShippingSale", filter);
                return new PageResult<OPC_ShippingSale>(shipSales, shipSales.Count);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*根据销售单拿到订单*/

        public PageResult<Order> SearchOrderBySale(string orderNo)
        {
            try
            {
                var order = RestClient.GetSingle<Order>("order/GetOrderByOderNo", string.Format("orderNo={0}", orderNo));
                return new PageResult<Order>(new List<Order> {order}, 100);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*完成打印销售单 状态*/ //SetSaleOrderPrintSale

        public bool SetStatusAffirmPrintSaleFinish(IList<string> saleOrderNoList)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderFinishPrintSale", saleOrderNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*设置销售单入库 状态*/

        public bool SetStatusStoreInSure(IList<string> saleOrderNoList)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderShipInStorage", saleOrderNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*设置销售单出库 状态*/

        /*设置销售单缺货 状态*/

        public bool SetStatusSoldOut(IList<string> saleOrderNoList)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderStockOut", saleOrderNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<OPC_ShippingSale> GetListShipSaleBySale(string saleOrderNo)
        {
            try
            {
                var shipSale = RestClient.GetSingle<OPC_ShippingSale>("trans/GetShippingSaleByOrderNo",
                    string.Format("saleNo={0}", saleOrderNo));
                return shipSale == null ? new List<OPC_ShippingSale>() : new List<OPC_ShippingSale> {shipSale};
            }
            catch (Exception ex)
            {
                return new List<OPC_ShippingSale>();
            }
        }

        public PageResult<OPC_SaleDetail> SelectSaleDetail(string saleOrderNo)
        {
            try
            {
                IList<OPC_SaleDetail> lst = RestClient.Get<OPC_SaleDetail>("sale/GetSaleOrderDetails",
                    string.Format("saleOrderNo={0}", saleOrderNo));
                return new PageResult<OPC_SaleDetail>(lst, 100);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool SetStatusPrintInvoice(IList<string> saleOrderNoList)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderPrintInvoice", saleOrderNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool SetStatusPrintExpress(string shipCode)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderPrintExpress", shipCode);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*打印销售单*/

        public bool ExecutePrintSale(IList<string> saleOrderNoList)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderPrintSale", saleOrderNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool SaveShip(ShippingSaleCreateDto dto)
        {
            try
            {
                return RestClient.Post("trans/CreateShippingSale", dto);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<OPC_Sale> SelectSaleByShip(string shipCode)
        {
            try
            {
                List<OPC_Sale> lst = RestClient.Get<OPC_Sale>("trans/GetSaleByShippingSaleNo",
                    string.Format("shippingSaleNo={0}", shipCode)).ToList();
                return lst;
            }
            catch (Exception ex)
            {
                return new List<OPC_Sale>();
            }
        }


        public bool SetSaleOrderShipped(IList<string> shipSalerNoList)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderShipped", shipSalerNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PrintSale(IList<string> saleOrderNoList)
        {
            try
            {
                return RestClient.Put("sale/PrintSale", saleOrderNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetSaleOrderStockOut(IList<string> saleOrderNoList)
        {
            try
            {
                return RestClient.Put("sale/SetSaleOrderStockOut", saleOrderNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}