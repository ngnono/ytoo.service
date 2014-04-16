using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Customer;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Customer
{
    [Export(typeof (ICustomerReturnGoods))]
    public class CustomerReturnGoodsService : ICustomerReturnGoods
    {
        public IList<OPC_SaleRMA> ReturnGoodsSearch(ReturnGoodsGet returnGoodsGet)
        {
            try
            {
                PageResult<OPC_SaleRMA> lst = RestClient.GetPage<OPC_SaleRMA>("custom/GetOrder",
                    returnGoodsGet.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OPC_SaleRMA>();
            }
        }

        public IList<OrderItem> GetOrderDetailByOrderNo(string orderNO)
        {
            try
            {
                PageResult<OrderItem> lst = RestClient.GetPage<OrderItem>("order/GetOrderItemsByOrderNo",
                    string.Format("orderNo={0}&pageIndex={1}&pageSize={2}", orderNO, 1, 300));
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OrderItem>();
            }
        }

        public bool CustomerReturnGoodsSave(RMAPost rmaPost)
        {
            try
            {
                return RestClient.Post("rma/CreateSaleRMA", rmaPost);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region 自主退货
        //自主退货明细查询
        public IList<OrderItem> GetOrderDetailByOrderNoWithSelf(string orderNo)
        {
            try
            {
                PageResult<OrderItem> lst = RestClient.GetPage<OrderItem>("custom/GetOrderItemsByOrderNoAutoBack",
                    string.Format("orderNo={0}&pageIndex={1}&pageSize={2}", orderNo, 1, 300));
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OrderItem>();
            }
        }
        //自主退货明细查询
        public IList<OPC_SaleRMA> ReturnGoodsSearchForSelf(ReturnGoodsGet returnGoodsGet)
        {
            try
            {
                PageResult<OPC_SaleRMA> lst = RestClient.GetPage<OPC_SaleRMA>("custom/GetOrderAutoBack",
                    returnGoodsGet.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OPC_SaleRMA>();
            }
        }
        //自助退货  退货审核通过
        public bool CustomerReturnGoodsSelfPass(RMAPost rmaPost)
        {
            try
            {
                return RestClient.Post("rma/CreateSaleRmaAuto", rmaPost);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //自助退货 拒绝退货申请
        public bool CustomerReturnGoodsSelfReject(RMAPost rmaPost)
        {
            try
            {//接口不对
                return RestClient.Post("rma/CreateSaleRMA", rmaPost);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        }

        #endregion
    }
