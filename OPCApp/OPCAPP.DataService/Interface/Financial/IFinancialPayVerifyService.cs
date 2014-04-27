using System.Collections.Generic;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto.Financial;

namespace OPCApp.DataService.Interface.Financial
{
    public interface IFinancialPayVerify
    {
        //退货付款确认  
        bool ReturnGoodsPayVerify(string rmaNo, decimal money);
        IList<RMADto> GetRmaByRmaOder(string rmaNo);
        IList<SaleRmaDto> GetRmaByReturnGoodPay(ReturnGoodsPayDto returnGoodsPay);

        bool FinancialVerifyPass(List<string> rmaNoList);
        bool FinancialVerifyNoPass(List<string> rmaNoList);

        //退回付款确认 查询 退货单列表
        IList<RMADto> GetRmaByFilter(PackageReceiveDto packageReceive);

        // 网上收银流水对账查询

        IList<WebSiteCashierSearchDto> GetCashierStatistics(SearchCashierDto searchCashierDto);

        // 网站退货明细统计

        IList<WebSiteReturnGoodsStatisticsDto> GetReturnGoodsStatistics(SearchStatistics searchStatistics);
        //网站销售明细统计

        IList<WebSiteSalesStatisticsDto> GetSalesStatistics(SearchStatistics searchStatistics);
    }
}