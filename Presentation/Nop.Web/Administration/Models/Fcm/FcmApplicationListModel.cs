using Nop.Core.Domain.Fcm;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public partial class FcmApplicationListModel : BaseNopModel
    {
        public FcmApplicationListModel()
        {
            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Application.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Admin.Application.List.SearchId")]
        [AllowHtml]
        public int SearchId { get; set; }

        [NopResourceDisplayName("Admin.Application.List.SearchPackage")]
        [AllowHtml]
        public string SearchPackage { get; set; }

        [NopResourceDisplayName("Admin.Application.List.SearchAppKey")]
        [AllowHtml]
        public string SearchAppKey { get; set; }

        [NopResourceDisplayName("Admin.Application.List.SearchFcmApplicationType")]
        public FcmApplicationType SearchFcmApplicationType { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Action.List.SearchVendorId")]
        public int SearchVendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

    }
}