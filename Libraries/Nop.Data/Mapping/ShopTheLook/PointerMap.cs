using Nop.Core.Domain.ShopTheLook;

namespace Nop.Data.Mapping.ShopTheLook
{
    public partial class PointerMap : NopEntityTypeConfiguration<Pointer>
    {
        public PointerMap()
        {
            this.ToTable("ShopTheLookPointer");
            this.HasKey(p => p.Id);
            this.Property(p => p.X).IsRequired().HasPrecision(18, 9);
            this.Property(p => p.Y).IsRequired().HasPrecision(18, 9);
            this.Property(p => p.TaggedProductId).IsRequired();
            this.Property(p => p.ProductId).IsRequired();
            this.Property(p => p.PictureId).IsRequired();
        }
    }
}
