using FluentValidation;
using Nop.Admin.Models.SMS;
using Nop.Core.Domain.SMS;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.SMS
{
    public partial class NumberAccountValidator : BaseNopValidator<NumberAccountModel>
    {
        public NumberAccountValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Number).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.AccountSid).NotEmpty();
            RuleFor(x => x.AuthToken).NotEmpty();

            SetDatabaseValidationRules<NumberAccount>(dbContext);
        }
    }
}