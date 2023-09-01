using FluentValidation;
using Nop.Admin.Models.Messages;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Interactive
{
    public class InteractiveFormAttributeValidator : BaseNopValidator<InteractiveFormAttributeModel>
    {
        public InteractiveFormAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.InteractiveForms.Attribute.Fields.Name.Required"));
            RuleFor(x => x.SystemName).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.InteractiveForms.Attribute.Fields.SystemName.Required"));
        }
    }
}