using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerActionMap : NopEntityTypeConfiguration<CustomerAction>
    {
        public CustomerActionMap()
        {
            this.ToTable("CustomerAction");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.Condition);
        }
    }
}
