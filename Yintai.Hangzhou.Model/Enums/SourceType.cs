using System.ComponentModel;
namespace Yintai.Hangzhou.Model.Enums
{
    public enum SourceType
    {
        [Description("����")]
        Default = 0,

        /// <summary>
        /// ��Ʒ
        /// </summary>
        [Description("��Ʒ")]
        Product = 1,

        /// <summary>
        /// �
        /// </summary>
        [Description("�")]
        Promotion = 2,

        /// <summary>
        /// ����
        /// </summary>
        [Description("����")]
        Comment = 3,

        /// <summary>
        /// �ͻ�Ф��LOGO�� ����Դʹ�ã�
        /// </summary>
        [Description("�û�ͼƬ")]
        CustomerPortrait = 4,

        /// <summary>
        /// Ʒ��logo ����Դʹ�ã�
        /// </summary>
        [Description("Ʒ��logo")]
        BrandLogo = 5,

        /// <summary>
        /// ����LOGO����Դʹ�ã�
        /// </summary>
        [Description("�ٻ���logo")]
        StoreLogo = 6,

        /// <summary>
        /// ����
        /// </summary>
        Store = 7,

        /// <summary>
        /// �ͻ�
        /// </summary>
        Customer = 8,

        /// <summary>
        /// ר��
        /// </summary>
        [Description("ר��")]
        SpecialTopic = 9,

        [Description("��������")]
        CommentAudio = 10,

        BannerPromotion = 11,
        [Description("�û�����ͼ")]
        CustomerThumbBackground = 12,
        //ProductAudio = 12,

        PMessage = 13,

        GiftCard = 14,
        Combo = 15,
        Inventory = 100
    }
}