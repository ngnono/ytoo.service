using System;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IShippingSaleRepository : IRepository<OPC_ShippingSale>
    {
        OPC_ShippingSale GetBySaleOrderNo(string saleNo);

        PageResult<OPC_ShippingSale> GetByShippingCode(string shippingCode, int pageIndex, int pageSize = 20);

       
        PageResult<OPC_ShippingSale> Get(string shippingCode, DateTime startTime, DateTime endTime, int shippingStatus,
            int pageIndex, int pageSize = 20);

        /// <summary>
        /// 获得发货单
        /// </summary>
        /// <param name="saleOrderNo">销售单编号</param>
        /// <param name="expressNo">快递单号.</param>
        /// <param name="startGoodsOutDate">发货时间1</param>
        /// <param name="endGoodsOutDate">发货时间2</param>
        /// <param name="outGoodsCode">The out goods code.</param>
        /// <param name="sectionId">专柜ID</param>
        /// <param name="shippingStatus">The shipping status.</param>
        /// <param name="customerPhone">客户电话</param>
        /// <param name="brandId">品牌ID</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>PageResult{OPC_ShippingSale}.</returns>
        PageResult<OPC_ShippingSale> GetShippingSale(string saleOrderNo, string expressNo, DateTime startGoodsOutDate,
            DateTime endGoodsOutDate,
            string outGoodsCode, int sectionId, int shippingStatus, string customerPhone, int brandId, int pageIndex,
            int pageSize);

        OPC_ShippingSale GetByRmaNo(string rmaNo);

        PageResult<OPC_ShippingSale> GetByOrderNo(string orderNo, DateTime startDate, DateTime endDate, int pageIndex,
            int pageSize,int shippingStatus);
    }
}