using System;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Dto.Financial
{
    /// <summary>
    /// 网上收银流水对账查询
    /// </summary>
    public class SearchCashierRequest
    {

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? StoreId { get; set; }
        public string PayType { get; set; }
        public string FinancialType { get; set; }

        public void FormatDate()
        {
            StartTime = StartTime.Date;
            EndTime = EndTime.Date.AddDays(1);
        }
    }
}