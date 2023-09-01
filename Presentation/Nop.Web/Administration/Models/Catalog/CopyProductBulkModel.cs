using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class CopyProductBulkModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.Products.Copy.FromVendor")]
        public int FromVendor { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.ToVendor")]
        public int ToVendor { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.CopyImages")]
        public bool CopyImages { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.Published")]
        public bool Published { get; set; }
    }
}