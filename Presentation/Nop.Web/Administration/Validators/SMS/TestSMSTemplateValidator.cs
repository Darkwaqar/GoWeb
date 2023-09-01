using FluentValidation;
using Nop.Admin.Models.SMS;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.SMS
{
    public partial class TestSMSTemplateValidator : BaseNopValidator<TestSMSTemplateModel>
    {
        public TestSMSTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SendTo).NotEmpty();
        }
    }
}