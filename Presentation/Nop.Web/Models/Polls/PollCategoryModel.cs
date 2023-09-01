using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Polls
{
    public partial class PollCategoryModel : BaseNopEntityModel
    {
        public PollCategoryModel()
        {
            PictureModel = new PictureModel();
            AlternativePictureModel = new PictureModel();
            Polls = new List<PollModel>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public PictureModel PictureModel { get; set; }
        public PictureModel AlternativePictureModel { get; set; }
        public bool DisplayPollCategoryBreadcrumb { get; set; }
        public bool Collection { get; set; }
        public string CollectionName { get; set; }
        public string CollectionDiscription { get; set; }
        public PictureModel CollectionPricture { get; set; }

       
        public IList<PollModel> Polls { get; set; }
        public object SelectedStoreIds { get; internal set; }
       
    }
}