using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SMS
{
    public partial class QueuedSMSListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.QueuedSMS.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.List.FromSMS")]
        [AllowHtml]
        public string SearchFromSMS { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.List.ToSMS")]
        [AllowHtml]
        public string SearchToSMS { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.List.LoadNotSent")]
        public bool SearchLoadNotSent { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.List.MaxSentTries")]
        public int SearchMaxSentTries { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.List.GoDirectlyToNumber")]
        public int GoDirectlyToNumber { get; set; }
    }
}