using FluentValidation;
using Nop.Admin.Models.Magazines;
using Nop.Core.Domain.Magazines;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Magazines
{
    public class MagazineValidator : BaseNopValidator<MagazineModel>
    {
        public MagazineValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Magazine.Fields.Name.Required"));
            RuleFor(x => x.PictureId).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Magazine.Fields.PictureId.Required"));
            SetDatabaseValidationRules<Magazine>(dbContext);
        }
    }
}