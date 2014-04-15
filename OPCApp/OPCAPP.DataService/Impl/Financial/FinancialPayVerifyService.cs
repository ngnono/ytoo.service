using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Financial;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto.Financial;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (IFinancialPayVerify))]
    public class FinancialPayVerifyService : IFinancialPayVerify
    {
        public bool ReturnGoodsPayVerify(string rmaNo, decimal money)
        {
            try
            {
                return RestClient.Post("rma/CompensateVerify", new {RmaNo = rmaNo, Money = money});
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
                PageResult<RMADto> lst = RestClient.Get<RMADto>("rma/GetByRmaNo", string.Format("RmaNo={0}", rmaNo), 1,
                    300);
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }

        public IList<SaleRmaDto> GetRmaByReturnGoodPay(ReturnGoodsPayDto returnGoodsPay)
        {
            try
            {
                PageResult<SaleRmaDto> lst = RestClient.GetPage<SaleRmaDto>("rma/GetRmaByReturnGoodPay",
                    returnGoodsPay.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<SaleRmaDto>();
            }
        }


        public bool FinancialVerifyPass(List<string> rmaNoList)
        {
            try
            {
                return RestClient.Post("rma/FinaceVerify", new {RmaNos = rmaNoList, Pass = true});
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool FinancialVerifyNoPass(List<string> rmaNoList)
        {
            try
            {
                return RestClient.Post("rma/FinaceVerify", new {RmaNos = rmaNoList, Pass = false});
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IList<RMADto> GetRmaByFilter(PackageReceiveDto packageReceive)
        {
            try
            {
                PageResult<RMADto> lst = RestClient.GetPage<RMADto>("rma/GetByFinaceDto", packageReceive.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }

        //

        // 网上收银流水对账查询

        public IList<WebSiteCashierSearchDto> GetCashierStatistics(SearchCashierDto searchCashierDto)
        {
            try
            {
                PageResult<WebSiteCashierSearchDto> lst =
                    RestClient.GetPage<WebSiteCashierSearchDto>("order/WebSiteCashier", searchCashierDto.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<WebSiteCashierSearchDto>();
            }
        }

        // 网站退货明细统计

        public IList<WebSiteReturnGoodsStatisticsDto> GetReturnGoodsStatistics(SearchStatistics searchStatistics)
        {
            try
            {
                PageResult<WebSiteReturnGoodsStatisticsDto> lst =
                    RestClient.GetPage<WebSiteReturnGoodsStatisticsDto>("order/WebSiteStatReturnDetail",
                        searchStatistics.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<WebSiteReturnGoodsStatisticsDto>();
            }
        }

        //网站销售明细统计

        public IList<WebSiteSalesStatisticsDto> GetSalesStatistics(SearchStatistics searchStatistics)
        {
            try
            {
                PageResult<WebSiteSalesStatisticsDto> lst =
                    RestClient.GetPage<WebSiteSalesStatisticsDto>("order/WebSiteStatSaleDetail", searchStatistics.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<WebSiteSalesStatisticsDto>();
            }
        }
    }
}