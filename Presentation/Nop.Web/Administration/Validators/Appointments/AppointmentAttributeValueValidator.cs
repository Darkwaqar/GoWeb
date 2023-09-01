using FluentValidation;
using Nop.Admin.Models.Appointments;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Appointments
{
    public partial class AppointmentAttributeValueValidator : BaseNopValidator<AppointmentAttributeValueModel>
    {
        public AppointmentAttributeValueValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.AppointmentAttributes.Values.Fields.Name.Required"));

            SetDatabaseValidationRules<AppointmentAttributeValueModel>(dbContext);
        }
    }
}