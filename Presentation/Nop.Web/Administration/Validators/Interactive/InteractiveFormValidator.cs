using FluentValidation;
using Nop.Admin.Models.Messages;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Interactive
{
    public class InteractiveFormValidator : BaseNopValidator<InteractiveFormModel>
    {
        public InteractiveFormValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.InteractiveForms.Fields.Name.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.InteractiveForms.Fields.Body.Required"));
        }
    }
}