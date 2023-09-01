using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.MServices.Models.Product
{
    public class ProductModelAPI
    {
        public ProductModelAPI()
        {
            VendorId = 0;
            VendorImage = "";
            VendorName = "";
        }

        /// <summary>
        /// Gets or sets the product type identifier
        /// </summary>
        public int ProductTypeId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string formattedPrice { get; set; }
        public decimal price { get; set; }
        public string formattedOldPrice { get; set; }
        public decimal oldPrice { get; set; }
        public string formattedSpecialPrice { get; set; }
        public decimal specialPrice { get; set; }
        public string formattedPriceWithDiscount { get; set; }
        public decimal priceWithDiscount { get; set; }
        public string thumbUrl { get; set; }
        public string currencyCode { get; set; }

        /// <summary>
        /// Gets or sets the product type
        /// </summary>
        public ProductType ProductType
        {
            get { return (ProductType)this.ProductTypeId; }
            set { this.ProductTypeId = (int)value; }
        }

        public int VendorId { get;  set; }
        public string VendorImage { get; set; }
        public string VendorName { get; set; }
    }

    public class ProductCompactModelAPI
    {
        public int id { get; set; }
        public string thumbUrl { get; set; }
        public string title { get; set; }
    }

    public class ProductModelAPIIOS
    {
        public ProductModelAPIIOS()
        {
            associatedProducts = new List<ProductModelAPIIOS>();
        }
        public int id { get; set; }
        public string title { get; set; }
        public string formattedPrice { get; set; }
        public decimal price { get; set; }
        public string thumbUrl { get; set; }
        public string shortDescription { get; set; }
        public string ProductTag { get; set; }
        public List<ProductModelAPIIOS> associatedProducts { get; set; }
    }
}