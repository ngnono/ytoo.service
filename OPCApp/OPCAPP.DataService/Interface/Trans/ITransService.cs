using System.Collections.Generic;
using OPCApp.Domain.Dto;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ITransService
    {
        bool ExecutePrintSale(IList<string> saleOrderNoList);
        bool SetStatusAffirmPrintSaleFinish(IList<string> saleOrderNoList);
        bool SetStatusStoreInSure(IList<string> saleOrderNoList);
        bool SetStatusSoldOut(IList<string> saleOrderNoList);

        /// <summary>
        ///     快递单完成打应
        /// </summary>
        /// <param name="goodsOutCode">
        ///     即是对应数据库shipCode  对应客户端数据转换类goodsOutCode
        ///     快递单号 或者 发货单号 它们是一个值
        /// </param>
        /// <returns></returns>
        bool SetStatusPrintExpress(string goodsOutCode);

        bool SetSaleOrderShipped(IList<string> goodOutNoList);
        bool SetStatusPrintInvoice(IList<string> saleOrderNoList);
        PageResult<OPC_Sale> Search(string salesfilter, EnumSearchSaleStatus searchSaleStatus);
        PageResult<Order> SearchOrderBySale(string saleOrder);
        List<OPC_ShippingSale> GetListShipSaleBySale(string saleOrder);
        PageResult<OPC_SaleDetail> SelectSaleDetail(string saleOrderNo);

        bool SaveShip(ShippingSaleCreateDto dto);
        PageResult<OPC_ShippingSale> GetListShip(string filter);

        List<OPC_Sale> SelectSaleByShip(string shipCode);
    }
}