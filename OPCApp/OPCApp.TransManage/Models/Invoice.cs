using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.TransManage.Models
{
    public class Invoice
    {
        public string SID { get; set; }

        public string DDCode { get; set; }

        public string QDDDCode { get; set; }

        public string ZFF { get; set; }

        public string YFKZE { get; set; }

        public string MDYF { get; set; }
    }

    public class Invoice4Get
    {
        public string IfEnableDate { get; set; }//是否时间条件有效

        public string IfEnableDD { get; set; }//是否订单号条件有效

        public string QDDDCode { get; set; }//开始时间条件

        public string ZFF { get; set; }//结束时间条件

        public string YFKZE { get; set; }//订单号

    }

    public class InvoiceDetail
    {

    }

    public class InvoiceDetail4Get
    {
        public string X { get; set; }//选中的销售单ID
    }
}
