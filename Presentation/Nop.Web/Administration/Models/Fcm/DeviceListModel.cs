using Nop.Core.Domain.Fcm;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public partial class DeviceListModel : BaseNopModel
    {
        public DeviceListModel()
        {
            AvailableApplications = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Device.List.SearchBrand")]
        [AllowHtml]
        public string SearchBrand { get; set; }

        [NopResourceDisplayName("Admin.Device.List.SearchCarrier")]
        [AllowHtml]
        public string SearchCarrier { get; set; }

        [NopResourceDisplayName("Admin.Device.List.SearchFcmApplicationType")]
        public FcmApplicationType SearchFcmApplicationType { get; set; }

        [NopResourceDisplayName("Admin.Device.List.SearchLongitude")]
        [AllowHtml]
        public decimal SearchLongitude { get; set; }

        [NopResourceDisplayName("Admin.Device.List.SearchLatitude")]
        [AllowHtml]
        public decimal SearchLatitude { get; set; }

        [NopResourceDisplayName("Admin.Device.List.SearchHidden")]
        public bool SearchHidden { get; set; }

        public string SearchPackage { get; set; }

        public IList<SelectListItem> AvailableApplications { get;  set; }
    }
}