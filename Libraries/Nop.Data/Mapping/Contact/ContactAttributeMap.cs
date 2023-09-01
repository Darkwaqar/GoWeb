using Nop.Core.Domain.Contact;

namespace Nop.Data.Mapping.Contact
{
    public partial class ContactAttributeMap : NopEntityTypeConfiguration<ContactAttribute>
    {
        public ContactAttributeMap()
        {
            this.ToTable("ContactAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.AttributeControlType);
        }
    }
}
