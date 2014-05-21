using System;
using OPCApp.Domain.Attributes;
using OPCApp.Domain.Enums;

namespace OPCApp.Domain.Models
{
    [Uri("salesorder")]
    public class OPC_Sale : Model
    {
        public OPC_Sale()
        {
            SaleStatus = "������";
        }

        #region Properties generated from DB

        public string OrderNo { get; set; }
        public string SaleOrderNo { get; set; }
        public string SalesType { get; set; }
        public string ShipViaId { get; set; }
        public EnumSaleOrderStatus Status { get; set; }
        public string ShippingCode { get; set; }
        public decimal ShippingFee { get; set; }
        public int? ShippingStatus { get; set; }
        public string ShippingRemark { get; set; }
        public DateTime SellDate { get; set; }
        public string IfTrans { get; set; }
        public string TransStatus { get; set; }
        public decimal SalesAmount { get; set; }
        public int? SalesCount { get; set; }
        public string CashStatusName { get; set; }
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

        #endregion

        #region Added by Frank

        public int OrderId { get; set; }

        public int? ShippingSaleId { get; set; }

        public Order Order { get; set; }

        public Counter Counter { get; set; }

        public OPC_ShippingSale DeliveryOrder { get; set; }

        #endregion

        #region Properties to delete

        public string SectionName { get; set; }

        //������
        public string ReceivePerson { get; set; }

        public string OrderSource { get; set; }

        public string SaleStatus { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string TransNo { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string TransName { get; set; }

        public string StatusName { get; set; }

        /// <summary>
        /// �ŵ�
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// ��Ʊ̧ͷ
        /// </summary>
        public string InvoiceSubject { get; set; }

        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// �ŵ�绰
        /// </summary>
        public string StoreTelephone { get; set; }

        /// <summary>
        /// �ŵ��ַ
        /// </summary>
        public string StoreAddress { get; set; }

        #endregion
    }
}