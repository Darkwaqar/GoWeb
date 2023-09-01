using Nop.Core.Domain.Banners;

namespace Nop.Data.Mapping.Banners
{
    public partial class BannerPictureMap : NopEntityTypeConfiguration<BannerPicture>
    {
        public BannerPictureMap()
        {
            this.ToTable("Banner_Picture_Mapping");
            this.HasKey(pp => pp.Id);

            this.HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.Banner)
                .WithMany(p => p.BannerPictures)
                .HasForeignKey(pp => pp.BannerId);
        }
    }
}
