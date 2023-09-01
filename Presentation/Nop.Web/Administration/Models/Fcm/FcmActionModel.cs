using FluentValidation.Attributes;
using Nop.Admin.Validators.Fcm;
using Nop.Core.Domain.Fcm;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{
    [Validator(typeof(FcmActionValidator))]
    public class FcmActionModel : BaseNopEntityModel
    {
        public FcmActionModel()
        {
            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Fcm.Action.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Action.Fields.Extra")]
        [AllowHtml]
        public string Extra { get; set; }

        public FcmActionType FcmActionType { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Action.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Action.Fields.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }


        [NopResourceDisplayName("Admin.Fcm.Action.Fields.VendorId")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
    }
}