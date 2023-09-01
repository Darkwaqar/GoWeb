using FluentValidation;
using FluentValidation.Results;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Polls;
using Nop.Core.Domain.Polls;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Polls
{
    public partial class PollCategoryValidator : BaseNopValidator<PollCategoryModel>
    {
        public PollCategoryValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems"));
            Custom(x =>
            {
                if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                    return new ValidationFailure("PageSize", localizationService.GetResource("Admin.Catalog.Categories.Fields.PageSize.Positive"));

                return null;
            });

            SetDatabaseValidationRules<PollCategory>(dbContext);
        }
    }
}