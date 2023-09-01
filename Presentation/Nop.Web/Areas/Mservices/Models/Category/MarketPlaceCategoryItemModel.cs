namespace Nop.Web.Areas.Mservices.Models.Category
{
    public partial class MarketPlaceCategoryItemModel
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public int VendorId { get; set; }

        public int CategoryId { get; set; }

        public int ProductId { get; set; }

        public bool HasSubCategory { get; set; }
    }
}