using Nop.Core.Domain.SMS;

namespace Nop.Data.Mapping.SMS
{
    public partial class SMSTemplateMap : NopEntityTypeConfiguration<SMSTemplate>
    {
        public SMSTemplateMap()
        {
            this.ToTable("SMSTemplate");
            this.HasKey(mt => mt.Id);

            this.Property(mt => mt.Name).IsRequired().HasMaxLength(200);
            this.Property(mt => mt.BccNumberAddresses).HasMaxLength(200);
            this.Property(mt => mt.Subject).HasMaxLength(1000);
            this.Property(mt => mt.NumberAccountId).IsRequired();

            this.Ignore(mt => mt.DelayPeriod);
        }
    }
}