using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerReminderHistoryMap : NopEntityTypeConfiguration<CustomerReminderHistory>
    {
        public CustomerReminderHistoryMap()
        {
            this.ToTable("CustomerReminderHistory");
            this.HasKey(ca => ca.Id);
        }
    }
}
