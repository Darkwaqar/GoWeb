using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Banner
{
    public partial class BannerListModel : BaseNopModel
    {
        public BannerListModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Promotions.Banners.List.SearchBannerName")]
        [AllowHtml]
        public string SearchBannerName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.List.SearchVendor")]
        public int SearchVendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.List.ShowHidden")]
        public bool ShowHidden { get; set; }
       
    }
}