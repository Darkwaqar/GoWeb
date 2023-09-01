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
    [Validator(typeof(FcmApplicationValidator))]
    public class FcmApplicationModel : BaseNopEntityModel
    {
        public FcmApplicationModel()
        {
            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Fcm.Application.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Application.Fields.Package")]
        [AllowHtml]
        public string Package { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Application.Fields.AppKey")]
        [AllowHtml]
        public string AppKey { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Application.Fields.FcmApplicationType")]
        public FcmApplicationType FcmApplicationType { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Application.Fields.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        //vendor
        [NopResourceDisplayName("Admin.Fcm.Application.Fields.VendorId")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
    }
}