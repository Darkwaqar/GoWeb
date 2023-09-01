using FluentValidation;
using Nop.Admin.Models.Fcm;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Fcm
{
    public class LocationNotificationValidator : BaseNopValidator<LocationNotificationModel>
    {
        public LocationNotificationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Radius).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.Radius.Required"));
            RuleFor(x => x.Latitude).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.Latitude.Required"));
            RuleFor(x => x.Longitude).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.Longitude.Required"));
            RuleFor(x => x.SeletedId).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.SeletedId.Required"));
        }
    }
}