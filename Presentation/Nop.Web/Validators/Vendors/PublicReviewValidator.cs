using Nop.Web.Framework.Validators;
using Nop.Web.Models.Vendors;
using Nop.Services.Localization;
using FluentValidation;

namespace Nop.Web.Validators.Vendors
{
    public partial class PublicReviewValidator : BaseNopValidator<PublicReviewEditModel>
    {
        public PublicReviewValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddProductReviewModel.Title).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.Title.Required")).When(x => x.AddProductReviewModel != null);
            RuleFor(x => x.AddProductReviewModel.Title).Length(1, 200).WithMessage(string.Format(localizationService.GetResource("Reviews.Fields.Title.MaxLengthValidation"), 200)).When(x => x.AddProductReviewModel != null && !string.IsNullOrEmpty(x.AddProductReviewModel.Title));
            RuleFor(x => x.AddProductReviewModel.ReviewText).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.ReviewText.Required")).When(x => x.AddProductReviewModel != null);
            RuleFor(x => x.AddProductReviewModel.Rating).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.Rating.Required")).When(x => x.AddProductReviewModel != null);

            RuleFor(x => x.VendorReviewListModel.Title).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.Title.Required")).When(x => x.VendorReviewListModel != null);
            RuleFor(x => x.VendorReviewListModel.Title).Length(1, 200).WithMessage(string.Format(localizationService.GetResource("Reviews.Fields.Title.MaxLengthValidation"), 200)).When(x => x.VendorReviewListModel != null && !string.IsNullOrEmpty(x.VendorReviewListModel.Title));
            RuleFor(x => x.VendorReviewListModel.ReviewText).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.ReviewText.Required")).When(x => x.VendorReviewListModel != null);
            RuleFor(x => x.VendorReviewListModel.Rating).NotEmpty().WithMessage(localizationService.GetResource("Reviews.Fields.Rating.Required")).When(x => x.VendorReviewListModel != null);
        }
    }
}