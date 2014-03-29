namespace  OPCApp.Domain.Models
{
    public class OPC_RMADetail
    {
        public int Id { get; set; }

        /// <summary>
        ///     ���
        /// </summary>
        public string StyleNumber { get; set; }

        /// <summary>
        ///     ���
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        ///     ɫ��
        /// </summary>
        public string ColorNumber { get; set; }

        /// <summary>
        ///     �˻��۸�
        /// </summary>
        public decimal RMAPrice { get; set; }

        /// <summary>
        ///     �˻�����
        /// </summary>
        public int RMACount { get; set; }

        /// <summary>
        ///     Ʒ��
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        ///     ��Ʒ����
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        ///     ר����
        /// </summary>
        public string SectionId { get; set; }
    }
}