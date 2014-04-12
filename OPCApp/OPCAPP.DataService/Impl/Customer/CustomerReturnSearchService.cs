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
        public IList<OrderDto> ReturnGoodsSearch(ReturnGoodsInfoGet goodInfoGet)
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
                var lst=RestClient.Get<RMADto>("rma/GetByOrderNo", string.Format("orderNo={0}", orderNo));
                return lst;
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
                return RestClient.Post("custem/AgreeReturnGoods", rmaNos);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}