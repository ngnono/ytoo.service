namespace OPCApp.Domain.Models
{
    public class OPC_SaleDetail
    {
        public int Id { get; set; }

        /// <summary>
        ///     ��Ʒ����
        /// </summary>
        /// <value>The product no.</value>
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        ///     ���۵���
        /// </summary>
        /// <value>The sale order no.</value>
        public string SaleOrderNo { get; set; }

        /// <summary>
        ///     ���
        /// </summary>
        /// <value>The style no.</value>
        public string StyleNo { get; set; }

        /// <summary>
        ///     �ߴ�
        /// </summary>
        /// <value>The standard.</value>
        public string Size { get; set; }

        /// <summary>
        ///     ɫ��
        /// </summary>
        /// <value>The color.</value>
        public string Color { get; set; }

        /// <summary>
        ///     Ʒ��
        /// </summary>
        /// <value>The Brand.</value>
        public string Brand { get; set; }
        /// <summary>
        ///     ���ۼ۸�
        /// </summary>
        /// <value>The price.</value>
        public double SellPrice { get; set; }
        /// <summary>
        ///     ԭ��
        /// </summary>
        /// <value>The price.</value>
        public double Price { get; set; }
        /// <summary>
        ///     �����۸�
        /// </summary>
        /// <value>The sale price.</value>
        public double SalePrice { get; set; }

        /// <summary>
        ///     ��������
        /// </summary>
        /// <value>The sale count.</value>
        public int SellCount { get; set; }
        /// <summary>
        ///     �˻�����
        /// </summary>
        /// <value>The return count.</value>
        public int ReturnCount { get; set; }

        /// <summary>
        ///     ���Ƽ۸�
        /// </summary>
        /// <value>The label price.</value>
        public double LabelPrice { get; set; }

        /// <summary>
        ///     ��ע
        /// </summary>
        /// <value>The remark.</value>
        public string Remark { get; set; }

        public string SectionCode { get; set; }
        /// <summary>
        /// �ܼ�
        /// </summary>
        public double SumPrice { get; set; }
    }
}