using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class ReminderConditionEntityMap : NopEntityTypeConfiguration<ReminderConditionEntity>
    {
        public ReminderConditionEntityMap()
        {
            this.ToTable("ReminderConditionEntity");
            this.HasKey(ca => ca.Id);

            this.Property(sm => sm.EntityName).IsRequired().HasMaxLength(400);
        }
    }
}
