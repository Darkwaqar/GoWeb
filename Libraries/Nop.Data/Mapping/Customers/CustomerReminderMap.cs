using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerReminderMap : NopEntityTypeConfiguration<CustomerReminder>
    {
        public CustomerReminderMap()
        {
            this.ToTable("CustomerReminder");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.Condition);
        }
    }
}
