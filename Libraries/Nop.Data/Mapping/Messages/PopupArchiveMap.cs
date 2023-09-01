using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    public partial class PopupArchiveMap : NopEntityTypeConfiguration<PopupArchive>
    {
        public PopupArchiveMap()
        {
            this.ToTable("PopupArchive");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.PopupActiveId).IsRequired();
            this.Property(ea => ea.Name).IsRequired();
            this.Property(ea => ea.Body).IsRequired();
            this.Property(ea => ea.CreatedOnUtc).IsRequired();
        }
    }
}
