using System;
using System.Collections.Generic;
using OPCApp.Domain.Attributes;

namespace OPCApp.Domain.Models
{
    [Uri("deliveryorder")]
    public class OPC_ShippingSale : Model
    {
        public bool IsSelected { get; set; }

        /// <summary>
        ///     �˻�����
        /// </summary>
        public string RmaNo { get; set; }

        /// ����
        public string OrderNo { get; set; }

        public string SaleOrderNo { get; set; }

        /// ��������
        /// </summary>
        public string GoodsOutCode { get; set; }

        /// <summary>
        ///     ��ݵ���
        /// </summary>
        public string ExpressCode { get; set; }

        /// <summary>
        ///     ����״̬
        /// </summary>
        public string ShippingStatus { get; set; }

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
        ///     ����ʱ��
        /// </summary>
        public DateTime GoodsOutDate { get; set; }

        /// <summary>
        ///     ������ʽ
        /// </summary>
        public string GoodsOutType { get; set; }

        /// <summary>
        ///     ��ݹ�˾
        /// </summary>
        public string ShipCompanyName { get; set; }

        /// <summary>
        ///     ���Ա
        /// </summary>
        public string ShipManName { get; set; }

        /// <summary>
        ///     ��ӡ״̬
        /// </summary>
        public string PrintStatus { get; set; }

        /// <summary>
        ///     �ʱ�
        /// </summary>
        public string ShippingZipCode { get; set; }

        /// <summary>
        ///     ���ͷ�ʽ
        /// </summary>
        /// <value>The shipping method.</value>
        public string ShippingMethod { get; set; }

        /// <summary>
        ///     ֧����ݹ�˾��ݷ�
        /// </summary>
        /// <value>The express fee.</value>
        public double ShipViaExpressFee { get; set; }

        /// <summary>
        ///     ��ݷ�
        /// </summary>
        /// <value>The express fee.</value>
        public double ExpressFee { get; set; }

        public IList<OPC_Sale> SalesOrders { get; set; }
    }
}