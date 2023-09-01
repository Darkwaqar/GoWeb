using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Areas.Mservices.Models.Common
{
    public partial class SearchItem
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public int VendorId { get; set; }

        public int CategoryId { get; set; }

        public int ProductId { get; set; }

        public string VendorName { get; set; }

        public string VendorImage { get; set; }

    }
}