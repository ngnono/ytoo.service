using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (ICustomerInquiriesService))]
    public class CustomerReturnGoodsService : ICustomerReturnGoods
    {
       
        public IList<Order> ReturnGoodsSearch(Domain.Customer.RMAPost shipComment)
        {
            throw new NotImplementedException();
        }

        public bool CustomerReturnGoodsSave(Domain.Customer.ReturnGoodsGet shipComment)
        {
            throw new NotImplementedException();
        }
    }
}