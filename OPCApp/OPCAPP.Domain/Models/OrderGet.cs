using System;
using System.Collections.Generic;

namespace Intime.OPC.Domain.Models
{
    public class OrderGet
    {
        public string OrderNo { get; set; }
        public string OrderSource { get; set; }
        public int StoreId { get; set; }
        public int BrandId { get; set; }
        public int Status { get; set; }

        public string PaymentType { get; set; }
        public string OutGoodsType { get; set; }
        public string ShippingContactPhone { get; set; }
        public string ExpressDeliveryCode { get; set; }
        public string ExpressDeliveryCompany { get; set; }

        public OrderGet()
        {
            this.StartCreateDate = DateTime.Now;
            this.EndCreateDate = DateTime.Now;
        }
        public DateTime StartCreateDate
        {
            get;
            set;

        }
        //��ʼʱ������

        public DateTime EndCreateDate
        {
            get;
            set;
        }//����ʱ������
    }
}
