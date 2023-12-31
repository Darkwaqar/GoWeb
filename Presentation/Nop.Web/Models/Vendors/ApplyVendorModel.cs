﻿using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Vendors;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(ApplyVendorValidator))]
    public partial class ApplyVendorModel : BaseNopModel
    {
        public ApplyVendorModel()
        {
            VendorAttributes = new List<VendorAttributeModel>();
        }

        [NopResourceDisplayName("Vendors.ApplyAccount.Name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Vendors.ApplyAccount.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Vendors.ApplyAccount.Description")]
        public string Description { get; set; }

        public IList<VendorAttributeModel> VendorAttributes { get; set; }

        public string VendorAttributeInfo { get; set; }
        public string VendorAttributeXml { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool TermsOfServiceEnabled { get; set; }
        public bool TermsOfServicePopup { get; set; }

        public bool DisableFormInput { get; set; }
        public string Result { get; set; }
    }

}