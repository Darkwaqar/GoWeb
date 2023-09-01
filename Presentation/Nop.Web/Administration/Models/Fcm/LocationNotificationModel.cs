using FluentValidation.Attributes;
using Nop.Admin.Validators.Fcm;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{

    [Validator(typeof(LocationNotificationValidator))]
    public class LocationNotificationModel : BaseNopModel
    {
        public LocationNotificationModel()
        {
            Notification = new NotificationModel() { SendImmediately = true };
        }
        [NopResourceDisplayName("Admin.Fcm.LocationNotification.Fields.Latitude")]
        public decimal Latitude { get; set; }

        [NopResourceDisplayName("Admin.Fcm.LocationNotification.Fields.Longitude")]
        public decimal Longitude { get; set; }

        [NopResourceDisplayName("Admin.Fcm.LocationNotification.Fields.Radius")]
        public int Radius { get; set; }

        [NopResourceDisplayName("Admin.Fcm.LocationNotification.Fields.SearchLocation")]
        [AllowHtml]
        public string SearchLocation { get; set; }

        public NotificationModel Notification { get; set; }

        [NopResourceDisplayName("Admin.Fcm.LocationNotification.Fields.SeletedId")]
        [AllowHtml]
        public string SeletedId { get; set; }

    }
}