using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorReviewsMap : NopEntityTypeConfiguration<VendorReview>
    {
        public VendorReviewsMap()
        {
            this.ToTable("VendorReviews");
            this.HasKey(m => m.Id);
            this.Property(m => m.CreatedOnUTC);
            this.Property(m => m.CustomerId);
            this.Property(m => m.HelpfulnessNoTotal);
            this.Property(m => m.HelpfulnessYesTotal);
            this.Property(m => m.IsApproved);
            this.Property(m => m.ProductId);
            this.Property(m => m.Rating);
            this.Property(m => m.ReviewText);
            this.Property(m => m.Title);
            this.Property(m => m.VendorId);
            this.Property(m => m.OrderId);
            this.Property(m => m.CertifiedBuyerReview);
            this.Property(m => m.DisplayCertifiedBadge);
        }
    }
}
