using Nop.Core.Domain.SMS;

namespace Nop.Data.Mapping.Messages
{
    public partial class QueuedSMSMap : NopEntityTypeConfiguration<QueuedSMS>
    {
        public QueuedSMSMap()
        {
            this.ToTable("QueuedSMS");
            this.HasKey(qe => qe.Id);

            this.Property(qe => qe.From).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.FromName).HasMaxLength(500);
            this.Property(qe => qe.To).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.ToName).HasMaxLength(500);
            this.Property(qe => qe.ReplyTo).HasMaxLength(500);
            this.Property(qe => qe.ReplyToName).HasMaxLength(500);
            this.Property(qe => qe.CC).HasMaxLength(500);
            this.Property(qe => qe.Bcc).HasMaxLength(500);
            this.Property(qe => qe.Subject).HasMaxLength(1000);


            this.Ignore(qe => qe.Priority);

            this.HasRequired(qe => qe.NumberAccount)
                .WithMany()
                .HasForeignKey(qe => qe.NumberAccountId).WillCascadeOnDelete(true);
        }
    }
}