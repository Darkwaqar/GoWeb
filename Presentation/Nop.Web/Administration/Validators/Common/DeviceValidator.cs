using FluentValidation;
using Nop.Admin.Models.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Common
{
    public partial class DeviceValidator : BaseNopValidator<DeviceModel>
    {
        public DeviceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DeviceId)
               .NotEmpty()
               .WithMessage(localizationService.GetResource("Admin.Device.Fields.DeviceId.Required"));
            RuleFor(x => x.Package)
              .NotEmpty()
              .WithMessage(localizationService.GetResource("Admin.Device.Fields.Package.Required"));
            RuleFor(x => x.DeviceOS)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Admin.Device.Fields.DeviceOS.Required"));
        }
    }
}