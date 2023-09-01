using Nop.Core.Domain.SMS;

namespace Nop.Data.Mapping.SMS
{
    public partial class NumberAccountMap : NopEntityTypeConfiguration<NumberAccount>
    {
        public NumberAccountMap()
        {
            this.ToTable("NumberAccount");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Number).IsRequired().HasMaxLength(255);
            this.Property(ea => ea.DisplayName).HasMaxLength(255);

            this.Ignore(ea => ea.FriendlyName);
        }
    }
}