using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public partial class QueuedFcmListModel : BaseNopModel
    {
        public QueuedFcmListModel()
        {
            AvailableVendors = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.FromFcm")]
        [AllowHtml]
        public string SearchFromFcm { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.ToFcm")]
        [AllowHtml]
        public string SearchToFcm { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.Package")]
        [AllowHtml]
        public string SearchPackage { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.AppKey")]
        [AllowHtml]
        public string SearchAppKey { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.LoadNotSent")]
        public bool SearchLoadNotSent { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.MaxSentTries")]
        public int SearchMaxSentTries { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.List.GoDirectlyToNumber")]
        public int GoDirectlyToNumber { get; set; }


        //vendor
        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.VendorId")]
        public int SearchVendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }


        //store mapping
        [NopResourceDisplayName("Admin.Fcm.Notification.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}