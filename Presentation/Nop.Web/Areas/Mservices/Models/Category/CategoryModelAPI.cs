using Nop.Web.MServices.Models.Product;
using System.Collections.Generic;

namespace Nop.Web.MServices.Models.Vendors
{
    public class CategoryModelAPI
    {
        public CategoryModelAPI()
        {
            productModel = new List<ProductCompactModelAPI>();
        }
        public int id { get; set; }
        public string thumbUrl { get; set; }
        public string CategoryName { get; set; }
        public string AlternativePictureUrl { get; set; }
        public List<ProductCompactModelAPI> productModel { get; set; }
        
    }
    public class CategoryCompactModelAPI
    {
        public CategoryCompactModelAPI()
        {
            Subcatagories = new List<SubCategoriesmodel>();
        }
        public int id { get; set; }
        public string title { get; set; }
        public string thumbUrl { get; set; }
        public string AlternativePictureUrl { get; set; }

        public List<SubCategoriesmodel> Subcatagories { get; set; }
        public int NumberOfProducts { get; internal set; }
    }

    public class SubCategoriesmodel
    {
        public int id { get; set; }

        public string title { get; set; }

        public string thumbUrl { get; set; }
        public string AlternativePictureUrl { get; internal set; }
        public int NumberOfProducts { get; internal set; }
    }

    public class CategoryCompactModelIOS
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}