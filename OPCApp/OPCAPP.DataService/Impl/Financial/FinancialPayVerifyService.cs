using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Financial;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (IFinancialPayVerify))]
    public class FinancialPayVerifyService : IFinancialPayVerify
    {
       
        public bool ReturnGoodsPayVerify(string rmaNo, decimal money)
        {
            try
            {
                return RestClient.Post("rma/CompensateVerify", new { RmaNo = rmaNo, Money =money});
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public IList<RMADto> GetRmaByRmaOder(string rmaNo)
        {
            try
            {
                var lst = RestClient.Get<RMADto>("rma/GetByRmaNo", string.Format("RmaNo={0}", rmaNo));
                return lst;
            }
            catch (Exception ex)
            {
                return new  List<RMADto>();
            }
        }
    }
}