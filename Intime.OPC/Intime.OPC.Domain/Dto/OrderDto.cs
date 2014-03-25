using System;

namespace Intime.OPC.Domain.Dto
{
    public class OrderDto
    {
        /// <summary>
        ///     ����ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     ������
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        ///     ����������
        /// </summary>
        public string OrderChannelNo { get; set; }

        /// <summary>
        ///     ֧����ʽ
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        ///     ������Դ
        /// </summary>
        public string OrderSouce { get; set; }

        /// <summary>
        ///     ����״̬
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        ///     ��Ʒ����
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///     ��Ʒ���
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        ///     �˿��˷�
        /// </summary>
        public decimal CustomerFreight { get; set; }

        /// <summary>
        ///     Ӧ����ϼ�
        /// </summary>
        public decimal MustPayTotal { get; set; }

        /// <summary>
        ///     ����ʱ��
        /// </summary>
        public DateTime BuyDate { get; set; }

        /// <summary>
        ///     �ջ�������
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        ///     �ջ��˵�ַ
        /// </summary>
        public string CustomerAddress { get; set; }

        /// <summary>
        ///     �ջ��˵绰
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        ///     �˿ͱ�ע
        /// </summary>
        public string CustomerRemark { get; set; }

        /// <summary>
        ///     �Ƿ�Ҫ��Ʊ
        /// </summary>
        public string IfReceipt { get; set; }

        /// <summary>
        ///     ��Ʊ̨ͷ
        /// </summary>
        public string ReceiptHead { get; set; }

        /// <summary>
        ///     ��Ʊ����
        /// </summary>
        public string ReceiptContent { get; set; }

        /// <summary>
        ///     ������ʽ
        /// </summary>
        public string OutGoodsType { get; set; }

        /// <summary>
        ///     �ʱ�
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        ///     ��������
        /// </summary>
        public string ShippingNo { get; set; }

        /// <summary>
        ///     ��ݵ���
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        ///     ��ݹ�˾
        /// </summary>
        public string ExpressCompany { get; set; }

        /// <summary>
        ///     ����ʱ��
        /// </summary>
        public DateTime OutGoodsDate { get; set; }
    }
}