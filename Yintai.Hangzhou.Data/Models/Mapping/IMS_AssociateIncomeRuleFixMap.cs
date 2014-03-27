using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public class IMS_AssociateIncomeRuleFixMap : EntityTypeConfiguration<IMS_AssociateIncomeRuleFix>
    {
        public IMS_AssociateIncomeRuleFixMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("IMS_AssociateIncomeRuleFix");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RuleId).HasColumnName("RuleId");
            this.Property(t => t.FixAmount).HasColumnName("FixAmount");
        }
    }
}
