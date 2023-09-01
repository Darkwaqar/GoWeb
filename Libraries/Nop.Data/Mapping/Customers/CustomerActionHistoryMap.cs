using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerActionHistoryMap : NopEntityTypeConfiguration<CustomerActionHistory>
    {
        public CustomerActionHistoryMap()
        {
            this.ToTable("CustomerActionHistory");
            this.HasKey(ca => ca.Id);
        }
    }
}
