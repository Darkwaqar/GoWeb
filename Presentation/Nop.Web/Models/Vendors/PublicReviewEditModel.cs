using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Vendors;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(PublicReviewValidator))]
    public partial class PublicReviewEditModel : BaseNopModel
    {
        public PublicReviewEditModel()
        {
            AddProductReviewModel = new AddProductReviewModel();
            VendorReviewListModel = new VendorReviewListModel();
        }

        public AddProductReviewModel AddProductReviewModel { get; set; }
        public VendorReviewListModel VendorReviewListModel { get; set; }
        public string VendorName { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public string VendorImageUrl { get; set; }
        public string ProductSeName { get; set; }
        public string VendorSeName { get; set; }
    }
}
