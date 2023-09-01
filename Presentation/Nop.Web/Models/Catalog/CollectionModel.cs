using System.Collections.Generic;
using Nop.Web.Models.Media;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class CollectionModel : BaseNopEntityModel
    {
        public CollectionModel()
        {
            Products = new List<ProductOverviewModel>();
        }

        public string Name { get; set; }
        public bool Collection { get; set; }
        public string CollectionName { get; set; }
        public string CollectionDiscription { get; set; }
        public PictureModel CollectionPicture { get; set; }
        public PictureModel CollectionLogo { get; set; }
        public PictureModel PictureModel { get; set; }
        public List<ProductOverviewModel> Products { get; set; }
    }
}