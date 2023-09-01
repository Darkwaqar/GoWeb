using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    public partial class QueuedFcmMap : NopEntityTypeConfiguration<QueuedFcm>
    {
        public QueuedFcmMap()
        {
            this.ToTable("QueuedFcm");
            this.HasKey(qe => qe.Id);

            this.Property(qe => qe.From).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.FromName).HasMaxLength(500);
            this.Property(qe => qe.DeviceId).IsRequired().HasMaxLength(500);

            this.Ignore(qe => qe.Priority);
            
        }
    }
}
