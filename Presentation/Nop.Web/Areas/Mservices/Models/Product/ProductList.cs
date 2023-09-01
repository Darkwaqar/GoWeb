using System.Collections.Generic;
using Nop.Web.Areas.MServices.Models.Product;
using Nop.Web.MServices.Models.Product;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Areas.Mservices.Models.Product
{
    public class ProductList
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public List<ProductModelAPI> Products { get; set; }
        public List<SpecificationAttributeModelAPI> SpecList { get; set; }
        public decimal? PriceMax { get; set; }
        public decimal? PriceMin { get; set; }
    }

    public class NewProductList
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public List<ProductOverviewModel> Products { get; set; }
        public List<SpecificationAttributeModelAPI> SpecList { get; set; }
        public decimal? PriceMax { get; set; }
        public decimal? PriceMin { get; set; }
    }
}