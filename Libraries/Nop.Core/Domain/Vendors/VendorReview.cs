using System;

namespace Nop.Core.Domain.Vendors
{
    public partial class VendorReview : BaseEntity
    {
        public  int VendorId { get; set; }
        public  int CustomerId { get; set; }
        public  int ProductId { get; set; }
        public  int OrderId { get; set; }
        public  bool IsApproved { get; set; }
        public  string Title { get; set; }
        public  string ReviewText { get; set; }
        public  int Rating { get; set; }
        public  int HelpfulnessYesTotal { get; set; }
        public  int HelpfulnessNoTotal { get; set; }
        public  DateTime CreatedOnUTC { get; set; }
        public  bool CertifiedBuyerReview { get; set; }
        public  bool DisplayCertifiedBadge { get; set; }
    }
}
