using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class CategoryListModel : BaseNopModel
    {
        public CategoryListModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableVendors= new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        [AllowHtml]
        public string SearchCategoryName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchVendor")]
        public int SearchVendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.ShowHidden")]
        public bool ShowHidden { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.ShowMarketplace")]
        public bool ShowMarketplace { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.ShowFeatured")]
        public bool ShowFeatured { get; set; }
    }
}