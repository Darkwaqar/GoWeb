using FluentValidation;
using Nop.Admin.Models.SMS;
using Nop.Core.Domain.SMS;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.SMS
{
    public partial class QueuedSMSValidator : BaseNopValidator<QueuedSMSModel>
    {
        public QueuedSMSValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.From).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.QueuedSMS.Fields.From.Required"));
            RuleFor(x => x.To).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.QueuedSMS.Fields.To.Required"));

            RuleFor(x => x.SentTries).NotNull().WithMessage(localizationService.GetResource("Admin.System.QueuedSMS.Fields.SentTries.Required"))
                                    .InclusiveBetween(0, 99999).WithMessage(localizationService.GetResource("Admin.System.QueuedSMS.Fields.SentTries.Range"));

            SetDatabaseValidationRules<QueuedSMS>(dbContext);

        }
    }
}