using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.TransManage.Models
{
    public class Invoice
    {
        public bool IsSelected { get; set; }


        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string SaleOrderNo { get; set; }
        public int SalesType { get; set; }
        public int? ShipViaId { get; set; }
        public int Status { get; set; }
        public int? ShippingCode { get; set; }
        public decimal ShippingFee { get; set; }
        public int? ShippingStatus { get; set; }
        public string ShippingRemark { get; set; }
        public DateTime SellDate { get; set; }
        public bool? IfTrans { get; set; }
        public int? TransStatus { get; set; }
        public decimal SalesAmount { get; set; }
        public int? SalesCount { get; set; }
        public int? CashStatus { get; set; }
        public string CashNum { get; set; }
        public DateTime? CashDate { get; set; }
        public int? SectionId { get; set; }
        public int? PrintTimes { get; set; }
        public string Remark { get; set; }
        public DateTime? RemarkDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
    }
    public class Invoice4Get
    {
        public Invoice4Get()
        {
            this.StartSellDate=DateTime.Now;
            this.EndSellDate = DateTime.Now;
        }
        public DateTime StartSellDate
        {
            get;
            set;

        }
        //开始时间条件

        public DateTime EndSellDate
        {
            get;
            set;
        }//结束时间条件

        public string OrderNo { get; set; }//订单号

        public string SaleOrderNo { get; set; }//销售单号

    }

    public class InvoiceDetail
    {

    }

    public class InvoiceDetail4Get
    {
        public string InvoiceID { get; set; }//选中的销售单ID
    }
}
