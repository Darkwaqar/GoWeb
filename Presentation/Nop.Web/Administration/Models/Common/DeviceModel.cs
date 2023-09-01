using FluentValidation.Attributes;
using Nop.Admin.Validators.Common;
using Nop.Core.Domain.Fcm;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Common
{

    [Validator(typeof(DeviceValidator))]
    public partial class DeviceModel : BaseNopEntityModel
    {

        [NopResourceDisplayName("Admin.Device.Fields.DeviceId")]
        public string DeviceId { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.Brand")]
        [AllowHtml]
        public string Brand { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.OSVersion")]
        [AllowHtml]
        public string OSVersion { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.Carrier")]
        [AllowHtml]
        public string Carrier { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.DeviceOS")]
        [AllowHtml]
        public FcmApplicationType DeviceOS { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.DeviceRam")]
        [AllowHtml]
        public string DeviceRam { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.Package")]
        [AllowHtml]
        public string Package { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.Longitude")]
        public decimal Longitude { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.Latitude")]
        public decimal Latitude { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.Error")]
        public string Error { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.Device.Fields.UpdatedOnUtc")]
        public DateTime UpdatedOnUtc { get; set; }

        //address in HTML format (usually used in grids)
        [NopResourceDisplayName("Admin.Device")]
        public string DeviceHtml { get; set; }
        public int CustomerId { get; internal set; }
    }
}