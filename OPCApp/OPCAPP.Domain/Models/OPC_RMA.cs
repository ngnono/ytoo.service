using System;

namespace  OPCApp.Domain.Models
{
    public class OPC_RMA
    {
        /// <summary>
        ///     �˻�ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     �˻�����
        /// </summary>
        public string RMANo { get; set; }

        /// <summary>
        ///     ������
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        ///     ���۵���
        /// </summary>
        public string SaleOrderNo { get; set; }

        /// <summary>
        ///     �ŵ�
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        ///     �˻�ԭ��
        /// </summary>
        public string RMAReason { get; set; }

        /// <summary>
        ///     �˻�״̬
        /// </summary>
        public string RMAStatus { get; set; }

        /// <summary>
        ///     �˻���״̬
        /// </summary>
        public string RMABillStatus { get; set; }

        /// <summary>
        ///     �˻�������״̬
        /// </summary>
        public string RMACashStatus { get; set; }

        /// <summary>
        ///     �����˻�ʱ��
        /// </summary>
        public DateTime RMAMustBackDate { get; set; }

        /// <summary>
        ///     �˻����
        /// </summary>
        public decimal RMAAmount { get; set; }

        /// <summary>
        ///     �˻�����
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     ������ˮ��
        /// </summary>
        public string CashNumber { get; set; }

        /// <summary>
        ///     ����ʱ��
        /// </summary>
        public DateTime CashDate { get; set; }

        /// <summary>
        ///     �˻�������ˮ��
        /// </summary>
        public string RMACashNumber { get; set; }

        /// <summary>
        ///     �˻�����ʱ��
        /// </summary>
        public DateTime RMACashDate { get; set; }

        /// <summary>
        ///     �˻�����
        /// </summary>
        public string RMAType { get; set; }

        /// <summary>
        ///     ר����
        /// </summary>
        public string SectionId { get; set; }

        /// <summary>
        ///     �˻�ʱ��
        /// </summary>
        public DateTime RMADate { get; set; }

        /// <summary>
        ///     ֧����ʽ
        /// </summary>
        public string CashType { get; set; }
    }
}