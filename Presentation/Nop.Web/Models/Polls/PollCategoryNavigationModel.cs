using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Polls
{
    public partial class PollCategoryNavigationModel : BaseNopModel
    {
        public PollCategoryNavigationModel()
        {
            PollCategories = new List<PollCategorySimpleModel>();
        }

        public int CurrentPollCategoryId { get; set; }
        public List<PollCategorySimpleModel> PollCategories { get; set; }
    }
}