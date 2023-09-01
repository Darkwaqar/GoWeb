using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class ActionConditionEntityMap : NopEntityTypeConfiguration<ActionConditionEntity>
    {
        public ActionConditionEntityMap()
        {
            this.ToTable("ActionConditionEntity");
            this.HasKey(ca => ca.Id);

            this.Property(sm => sm.EntityName).IsRequired().HasMaxLength(400);
        }
    }
}
