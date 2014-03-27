namespace Yintai.Hangzhou.Data.Models
{
    public partial class IMS_SectionOperator : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brands { get; set; }

        #region Overrides of BaseEntity

        /// <summary>
        /// KeyMemberId
        /// </summary>
        public override object EntityId
        {
            get { return Id; }

        }

        #endregion
    }
}
