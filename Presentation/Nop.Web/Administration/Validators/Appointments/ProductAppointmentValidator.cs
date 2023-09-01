using FluentValidation;
using Nop.Admin.Models.Appointments;
using Nop.Core;
using Nop.Core.Domain.Appointments;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Appointments
{
    public partial class ProductAppointmentValidator : BaseNopValidator<ProductAppointmentModel>
    {
        public ProductAppointmentValidator(ILocalizationService localizationService, IDbContext dbContext, IWorkContext workContext)
        {
            var isLoggedInAsVendor = workContext.CurrentVendor != null;
            //vendor can edit "Reply text" only
            if (!isLoggedInAsVendor)
            {
                RuleFor(x => x.AppointmentText).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ProductAppointments.Fields.AppointmentText.Required"));
            }

            SetDatabaseValidationRules<ProductAppointment>(dbContext);
        }
    }
}