using System.Collections.Generic;
using Nop.Core.Domain.Vendors;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Areas.Mservices.Models.Common
{
    public partial class AdvanceSearchModel
    {
        public AdvanceSearchModel()
        {
            Vendors = new List<SearchItem>();
            Categories = new List<SearchItem>();
            Products = new List<SearchItem>();
        }

        public List<SearchItem> Vendors { get; set; }
       
        public List<SearchItem> Categories { get; set; }

        public List<SearchItem> Products { get; set; }
    }
}