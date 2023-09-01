using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Common;
using System.Collections.Generic;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(WifeApplyValidator))]
    public partial class WifeApplyModel : BaseNopModel
    {
        public WifeApplyModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("WifeApply.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("WifeApply.BusinessName")]
        public string BusinessName { get; set; }

        [NopResourceDisplayName("WifeApply.ContactNumber")]
        public string ContactNumber { get; set; }

        [NopResourceDisplayName("WifeApply.Email")]
        public string Email { get; set; }


        [NopResourceDisplayName("WifeApply.Country")]
        public int CountryId { get; set; }

        [NopResourceDisplayName("WifeApply.StateProvinceId")]
        public string StateProvinceId { get; set; }

        [NopResourceDisplayName("WifeApply.City")]
        public string City { get; set; }

        [NopResourceDisplayName("WifeApply.SellOnline")]
        public bool SellOnline { get; set; }

        [NopResourceDisplayName("WifeApply.Website")]
        public string Website { get; set; }

        [NopResourceDisplayName("WifeApply.FacebookLink")]
        public string FacebookLink { get; set; }

        [NopResourceDisplayName("WifeApply.Physically")]
        public bool Physically { get; set; }

        [NopResourceDisplayName("WifeApply.ShopAddress")]
        public string ShopAddress { get; set; }

        [NopResourceDisplayName("WifeApply.TypeOfProducts")]
        public string TypeOfProducts { get; set; }

        [NopResourceDisplayName("WifeApply.ProductImage")]
        public int ProductImage { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("WifeApply.Enquiry")]
        public string Enquiry { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
    }
}