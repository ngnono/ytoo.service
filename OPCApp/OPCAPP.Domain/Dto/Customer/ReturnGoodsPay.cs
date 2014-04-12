using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OPCApp.Domain.Customer
{
    /// <summary>
    /// 退货付款确认
    /// </summary>
    public class ReturnGoodsPayDto
    {
        public ReturnGoodsPayDto()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string OrderNo { get; set; }


        public string PayType { get; set; }

        public int? StoreId { get; set; }
        public override string ToString()
        {

            return string.Format("StartDate={0}&EndDate={1}&OrderNo={2}&StoreId={3}&PayType={4}", StartDate, EndDate, OrderNo, StoreId, PayType);
        }
    }
}
