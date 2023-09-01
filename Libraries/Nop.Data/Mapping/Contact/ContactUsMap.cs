using Nop.Core.Domain.Contact;

namespace Nop.Data.Mapping.Contact
{
    public partial class ContactUsMap : NopEntityTypeConfiguration<ContactUs>
    {
        public ContactUsMap()
        {
            this.ToTable("ContactUs");
            this.HasKey(ea => ea.Id);
            this.Property(ea => ea.Subject).IsRequired();
            
        }
    }
}
