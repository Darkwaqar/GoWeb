using FluentValidation.Attributes;
using Nop.Admin.Validators.Fcm;
using Nop.Core.Domain.Fcm;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{

    [Validator(typeof(NotificationValidator))]
    public class NotificationModel : BaseNopEntityModel
    {
        public NotificationModel()
        {
            AvailableApplications = new List<SelectListItem>();
            AvailableActions = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }


        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.FcmType")]
        public FcmType FcmType { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.FcmApplicationType")]
        public FcmApplicationType FcmApplicationType { get; set; }


        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.Application")]
        public int ApplicationId { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.Action")]
        [AllowHtml]
        public int ActionId { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.Image")]
        [UIHint("Picture")]
        public int Image { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.DirectImageLink")]
        public string DirectImageLink { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.GotoLink")]
        public string GotoLink { get; set; }

        [NopResourceDisplayName("Admin.SendFcm.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.SendFcm.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }


        public IList<SelectListItem> AvailableApplications { get; set; }

        public IList<SelectListItem> AvailableActions { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.Icon")]
        [UIHint("Picture")]
        public int Icon { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.DirectIconLink")]
        public string DirectIconLink { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.UpdatedOnUtc")]
        public DateTime UpdatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.IsReaded")]
        public bool IsReaded { get; set; }


        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.ReadedOnUtc")]
        public DateTime? ReadedOnUtc { get; set; }

        //vendor
        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.VendorId")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }


        //store mapping
        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}