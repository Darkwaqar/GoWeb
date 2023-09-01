using FluentValidation;
using Nop.Admin.Models.Fcm;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;


namespace Nop.Admin.Validators.Messages
{
    public partial class QueuedFcmValidator : BaseNopValidator<QueuedFcmModel>
    {
        public QueuedFcmValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.From).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.QueuedFcms.Fields.From.Required"));
            RuleFor(x => x.DeviceId).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.QueuedFcms.Fields.DeviceId.Required"));

            RuleFor(x => x.SentTries).NotNull().WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.SentTries.Required"))
                                    .InclusiveBetween(0, 99999).WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.SentTries.Range"));

            SetDatabaseValidationRules<QueuedFcm>(dbContext);
        }
    }
}