using FluentValidation;
using Nop.Admin.Models.Fcm;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;

namespace Nop.Admin.Validators.Fcm
{
    public class NotificationValidator : BaseNopValidator<NotificationModel>
    {
        public NotificationValidator(ILocalizationService localizationService)
        {

            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.Title.Required"));
            RuleFor(x => x.Message).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.Message.Required"));
            RuleFor(x => x.Icon).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.Icon.Required"));
            RuleFor(x => x.GotoLink).NotEmpty().WithMessage(localizationService.GetResource("Admin.Fcm.Application.Fields.GotoLink.Required"))
                .When(x=>x.FcmType == Core.Domain.Fcm.FcmType.WebActivity);
            RuleFor(x => x.Image)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Fcm.Notification.Fields.Image.Required"))
                .When(x => String.IsNullOrWhiteSpace(x.DirectImageLink) && (x.FcmType == Core.Domain.Fcm.FcmType.Image || x.FcmType == Core.Domain.Fcm.FcmType.News));
            RuleFor(x => x.ActionId).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Fcm.Application.Fields.Intent.Required"))
                .When((x => x.FcmType == Core.Domain.Fcm.FcmType.Image || x.FcmType == Core.Domain.Fcm.FcmType.News));
        }
    }
}