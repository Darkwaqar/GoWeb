using FluentValidation;
using Nop.Services.Localization;
using Nop.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Interactive
{
    public class InteractiveFormAttributeValueValidator : BaseNopValidator<InteractiveFormAttributeValueModel>
    {
        public InteractiveFormAttributeValueValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.InteractiveForms.Attribute.Values.Fields.Name.Required"));
        }
    }
}