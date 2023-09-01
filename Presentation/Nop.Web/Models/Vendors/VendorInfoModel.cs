using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Vendors;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(VendorInfoValidator))]
    public class VendorInfoModel : BaseNopModel
    {
        public VendorInfoModel()
        {
            VendorAttributes = new List<VendorAttributeModel>();
        }
        [NopResourceDisplayName("Account.VendorInfo.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Picture")]
        public string PictureUrl { get; set; }



        //changes

        [UIHint("Picture")]
        //[NopResourceDisplayName("Account.VendorInfo.Picture")]
        public int PictureId { get; set; }

        [AllowHtml]
        public string AdminComment { get; set; }

        public bool Active { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [AllowHtml]
        public string ThemeColor { get; set; }

        [AllowHtml]
        public string Url { get; set; }

        [UIHint("Picture")]
        public int CoverPictureId { get; set; }

        public IList<VendorAttributeModel> VendorAttributes { get; set; }



    }
}