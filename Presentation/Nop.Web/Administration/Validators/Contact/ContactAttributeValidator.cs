using FluentValidation;
using Nop.Admin.Models.Contact;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Contact
{
    public class ContactAttributeValidator : BaseNopValidator<ContactAttributeModel>
    {
        public ContactAttributeValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ContactAttributes.Fields.Name.Required"));
            SetDatabaseValidationRules<ContactAttributeModel>(dbContext);
        }
    }
}