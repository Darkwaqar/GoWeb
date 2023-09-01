using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class GdprLogListModel : BaseNopModel
    {
        #region Ctor
        public GdprLogListModel()
        {
            AvailableRequestTypes = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }
        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Customers.GdprLog.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Admin.Customers.GdprLog.List.SearchRequestType")]
        public int SearchRequestTypeId { get; set; }

        [NopResourceDisplayName("Admin.Customers.GdprLog.List.StoreId")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableRequestTypes { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        #endregion
    }
}