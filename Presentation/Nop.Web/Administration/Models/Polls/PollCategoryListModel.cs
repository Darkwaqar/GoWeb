using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Polls
{
    public partial class PollCategoryListModel : BaseNopModel
    {
        public PollCategoryListModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableVendors= new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Poll.Categories.List.SearchPollCategoryName")]
        [AllowHtml]
        public string SearchPollCategoryName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Poll.Categories.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Poll.Categories.List.SearchVendor")]
        public int SearchVendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Poll.Categories.List.ShowHidden")]
        public bool ShowHidden { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Poll.Categories.List.ShowMarketplace")]
        public bool ShowMarketplace { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Poll.Categories.List.ShowFeatured")]
        public bool ShowFeatured { get; set; }
    }
}