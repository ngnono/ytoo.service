using System;

namespace OPCApp.Domain.Models
{
    public class OrderGet
    {
        public OrderGet()
        {
            StartCreateDate = DateTime.Now;
            EndCreateDate = DateTime.Now;
            Status = "-1";
            ExpressDeliveryCompany = "-1";
        }

        public string OrderNo { get; set; }
        public string OrderSource { get; set; }
        public string StoreId { get; set; }
        public string BrandId { get; set; }
        public string Status { get; set; }

        public string PaymentType { get; set; }
        public string OutGoodsType { get; set; }
        public string ShippingContactPhone { get; set; }
        public string ExpressDeliveryCode { get; set; }
        public string ExpressDeliveryCompany { get; set; }

        public DateTime StartCreateDate { get; set; }
        //��ʼʱ������

        public DateTime EndCreateDate { get; set; } //����ʱ������
    }
}