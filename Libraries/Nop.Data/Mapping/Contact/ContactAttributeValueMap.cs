using Nop.Core.Domain.Contact;

namespace Nop.Data.Mapping.Contact
{
    public partial class ContactAttributeValueMap : NopEntityTypeConfiguration<ContactAttributeValue>
    {
        public ContactAttributeValueMap()
        {
            this.ToTable("ContactAttributeValue");
            this.HasKey(cav => cav.Id);
            this.Property(cav => cav.Name).IsRequired().HasMaxLength(400);
            this.Property(cav => cav.ColorSquaresRgb).HasMaxLength(100);

            this.HasRequired(cav => cav.ContactAttribute)
                .WithMany(ca => ca.ContactAttributeValues)
                .HasForeignKey(cav => cav.ContactAttributeId);
        }
    }
}
