using FluentValidation;
using Nop.Admin.Models.Fcm;
using Nop.Core.Domain.Fcm;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Fcm
{
    public class FcmActionValidator : BaseNopValidator<FcmActionModel>
    {
        public FcmActionValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Action.Fields.Name.Required"));
            RuleFor(x => x.Extra).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Action.Fields.Extra.Required"));
            RuleFor(x => x.FcmActionType).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Action.Fields.FcmActionType.Required"));
            SetDatabaseValidationRules<FcmAction>(dbContext);
        }
    }
}