using FluentValidation;
using Nop.Admin.Models.Appointments;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Appointments
{
    public class AppointmentAttributeValidator : BaseNopValidator<AppointmentAttributeModel>
    {
        public AppointmentAttributeValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.AppointmentAttributes.Fields.Name.Required"));
            SetDatabaseValidationRules<AppointmentAttributeModel>(dbContext);
        }
    }
}