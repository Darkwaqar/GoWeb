using FluentValidation;
using Nop.Admin.Models.SMS;
using Nop.Core.Domain.SMS;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.SMS
{
    public partial class SMSTemplateValidator : BaseNopValidator<SMSTemplateModel>
    {
        public SMSTemplateValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Fields.Body.Required"));

            SetDatabaseValidationRules<SMSTemplate>(dbContext);
        }
    }
}