using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Polls
{
    public class PollCategorySimpleModel : BaseNopEntityModel
    {
        public PollCategorySimpleModel()
        {
            
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public bool IncludeInTopMenu { get; set; }
        
        public PictureModel PictureModel { get; set; }
    }
}