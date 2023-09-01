

using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public partial class FcmActionListModel : BaseNopModel
    {
        public FcmActionListModel()
        {
            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Fcm.Action.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Action.List.ShowHidden")]
        public bool ShowHidden { get; set; }

        [NopResourceDisplayName("Admin.Fcm.Action.List.SearchVendorId")]
        public int SearchVendorId { get;  set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
    }
}