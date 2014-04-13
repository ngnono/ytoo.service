using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.Domain.Customer;

namespace OPCApp.DataService.Customer
{
    [Export(typeof (ICustomerReturnSearch))]
    public class CustomerReturnSearchService : ICustomerReturnSearch
    {
        //laoda 物流退回 订单查询
        public IList<OrderDto> ReturnGoodsTransSearch(ReturnGoodsInfoGet goodInfoGet)
        {
            try
            {
                var lst = RestClient.GetPage<OrderDto>("order/GetByReturnTransGoodsInfo", goodInfoGet.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OrderDto>();
            }
        }
        //lao da 退货赔偿退回
        public IList<OrderDto> ReturnGoodsFinancialSearch(ReturnGoodsInfoGet goodInfoGet)
        {
            try
            {
                var lst = RestClient.GetPage<OrderDto>("order/GetByReturnFinancialGoodsInfo", goodInfoGet.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OrderDto>();
            }
        }
        public IList<OrderDto> ReturnGoodsRmaSearch(ReturnGoodsInfoGet goodInfoGet)
        {
            try
            {
                var lst = RestClient.GetPage<OrderDto>("order/GetByReturnGoodsInfo", goodInfoGet.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OrderDto>();
            }
        }

        public IList<RMADto> GetRmaByOrderNo(string orderNo)
        {
            try
            {
                var lst=RestClient.GetPage<RMADto>("rma/GetByOrderNo", string.Format("orderNo={0}&pageIndex={1}&pageSize={2}", orderNo,1,300));
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }


        public IList<RmaDetail> GetRmaDetailByRmaNo(string rmaNo)
        {
            try
            {
                IList<RmaDetail> lst = RestClient.Get<RmaDetail>("rma/GetRmaDetailByRmaNo",
                    string.Format("rmaNo={0}", rmaNo));
                return lst;
            }
            catch (Exception ex)
            {
                return new List<RmaDetail>();
            }
        }

        public bool AgreeReturnGoods(List<string> rmaNos)
        {
            try
            {
                return RestClient.Post("custom/AgreeReturnGoods", rmaNos);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}