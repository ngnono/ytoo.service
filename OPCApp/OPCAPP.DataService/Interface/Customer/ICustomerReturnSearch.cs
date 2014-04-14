﻿using System.Collections.Generic;
using OPCApp.Domain.Customer;

namespace OPCApp.DataService.Customer
{
    public interface ICustomerReturnSearch
    {
        //退货订单
        IList<OrderDto> ReturnGoodsRmaSearch(ReturnGoodsInfoGet goodInfoGet);
        //物流退回
        IList<OrderDto> ReturnGoodsTransSearch(ReturnGoodsInfoGet goodInfoGet);
        //赔偿退回
        IList<OrderDto> ReturnGoodsFinancialSearch(ReturnGoodsInfoGet goodInfoGet);
        IList<RMADto> GetRmaByOrderNo(string orderNo);
        IList<RmaDetail> GetRmaDetailByRmaNo(string rmaNo);
        bool AgreeReturnGoods(List<string> rmaNos);
        IList<RMADto> GetRmaTransByOrderNo(string orderNo);
        IList<RMADto> GetRmaFinancialByOrderNo(string orderNo);
    }
}