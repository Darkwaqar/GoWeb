using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Customers
{
    public partial class CustomerActionValidator : BaseNopValidator<CustomerActionModel>
    {
        public CustomerActionValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerAction.Fields.Name.Required"));

            SetDatabaseValidationRules<CustomerActionModel>(dbContext);
        }
    }
}