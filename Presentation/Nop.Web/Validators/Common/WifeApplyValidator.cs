using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Validators.Common
{
    public partial class WifeApplyValidator : BaseNopValidator<WifeApplyModel>
    {

        public WifeApplyValidator(ILocalizationService localizationService, CommonSettings commonSettings)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("WifeApply.Name.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("WifeApply.WrongEmail"));
            RuleFor(x => x.BusinessName).NotEmpty().WithMessage(localizationService.GetResource("WifeApply.BusinessName.Required"));
            RuleFor(x => x.ContactNumber).NotEmpty().WithMessage(localizationService.GetResource("WifeApply.ContactNumber.Required"));
            RuleFor(x => x.ContactNumber).Length(11).WithMessage(localizationService.GetResource("WifeApply.ContactNumber.Length"));
            RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResource("WifeApply.City.Required"));
            RuleFor(x => x.Enquiry).NotEmpty().WithMessage(localizationService.GetResource("ContactVendor.Enquiry.Required"));

        }
    }
}