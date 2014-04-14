using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCAPP.Domain.Dto.Financial
{
    //网上收银流水对账查询
  public  class SearchCashierDto
    {
        public SearchCashierDto()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;

        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int StoreId { get; set; }
        public string PayType { get; set; }
        public string FinancialType { get; set; }
        public override string ToString()
        {
            return string.Format("StartTime={0}&EndTime={1}&StoreId={2}&PayType={3}&FinancialType={4}&pageIndex={5}&pageSize={6}", StartTime, EndTime, StoreId, PayType, FinancialType, 1, 1000);
        }
    }
}
