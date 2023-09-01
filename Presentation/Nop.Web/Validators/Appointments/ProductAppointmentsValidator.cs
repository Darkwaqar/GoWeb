using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Appointment;

namespace Nop.Web.Validators.Appointments
{
    public class ProductAppointmentsValidator : BaseNopValidator<ProductAppointmentsModel>
    {
        public ProductAppointmentsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddProductAppointment.AppointmentText).NotEmpty().WithMessage(localizationService.GetResource("Appointments.Fields.AppointmentText.Required")).When(x => x.AddProductAppointment != null);
        }
    }
}