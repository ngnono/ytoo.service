using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OPCApp.Domain.Customer
{
    /// <summary>
    /// 客服服务-退货提醒 查询订单
    /// </summary>
    public class OutOfStockNotifyRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string OrderNo { get; set; }

        public string SaleOrderNo { get; set; }

        public int? SaleOrderStatus { get; set; }

        public string PayType { get; set; }

        public int? StoreId { get; set; }

        public void FormatDate()
        {
            StartDate = StartDate.Date;
            EndDate = EndDate.Date.AddDays(1);
        }
    }
}
