using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Banner
{
    public partial class BannerModel : BaseNopEntityModel
    {
        public BannerModel()
        {
            PictureModels = new List<BannerPictureModel>();
        }

        public string Name { get; set; }

        public string Body { get; set; }

        public int VendorId { get; set; }

        public bool Published { get; set; }

        public IList<BannerPictureModel> PictureModels { get; set; }

    }
}