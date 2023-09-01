using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    public partial class PopupActiveMap : NopEntityTypeConfiguration<PopupActive>
    {
        public PopupActiveMap()
        {
            this.ToTable("PopupActive");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Name).IsRequired();
            this.Property(ea => ea.Body).IsRequired();
            this.Property(ea => ea.CreatedOnUtc).IsRequired();
        }
    }
}
