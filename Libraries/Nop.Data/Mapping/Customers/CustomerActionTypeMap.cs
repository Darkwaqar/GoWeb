using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerActionTypeMap : NopEntityTypeConfiguration<CustomerActionType>
    {
        public CustomerActionTypeMap()
        {
            this.ToTable("CustomerActionType");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);
        }
    }
}
