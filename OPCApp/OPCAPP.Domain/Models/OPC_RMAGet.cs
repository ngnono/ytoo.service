namespace  OPCApp.Domain.Models
{
    public class OPC_RMAGet
    {
        /// <summary>
        ///     �����ջ���ʼʱ��
        /// </summary>
        public string StartGoodsGetDate { get; set; }

        /// <summary>
        ///     �����ջ�����ʱ��
        /// </summary>
        public string EndGoodsGetDate { get; set; }

        /// <summary>
        ///     ������
        /// </summary>
        public string OrderNo { get; set; }
    }
}