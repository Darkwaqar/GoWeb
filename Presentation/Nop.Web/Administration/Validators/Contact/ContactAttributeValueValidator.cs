using FluentValidation;
using Nop.Admin.Models.Contact;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Contact
{
    public partial class ContactAttributeValueValidator : BaseNopValidator<ContactAttributeValueModel>
    {
        public ContactAttributeValueValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ContactAttributes.Values.Fields.Name.Required"));

            SetDatabaseValidationRules<ContactAttributeValueModel>(dbContext);
        }
    }
}