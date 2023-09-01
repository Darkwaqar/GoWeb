using FluentValidation;
using Nop.Admin.Models.Fcm;
using Nop.Core.Domain.Fcm;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Fcm
{
    public class FcmApplicationValidator : BaseNopValidator<FcmApplicationModel>
    {
        public FcmApplicationValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Application.Fields.Name.Required"));
            RuleFor(x => x.Package).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Application.Fields.Package.Required"));
            RuleFor(x => x.AppKey).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Application.Fields.AppKey.Required"));

            SetDatabaseValidationRules<FcmApplication>(dbContext);
        }
    }
}