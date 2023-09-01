using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Web.Mvc;

namespace Nop.Admin.Models.Magazines
{
    public partial class MagazineListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Magazine.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Admin.Magazine.List.SearchDescription")]
        [AllowHtml]
        public string SearchDescription { get; set; }

        [NopResourceDisplayName("Admin.Magazine.List.SearchActive")]
        [AllowHtml]
        public bool SearchActive { get; set; }
       
    }
}