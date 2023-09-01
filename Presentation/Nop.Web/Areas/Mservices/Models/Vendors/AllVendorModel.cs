using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Web.Areas.Mservices.Models.Vendors
{
    public class AllVendorModel
    {
        public AllVendorModel()
        {
            All = new List<VendorMobileModel>();
            Brand = new List<VendorMobileModel>();
            Designer = new List<VendorMobileModel>();
            SheEarns = new List<VendorMobileModel>();
        }
        public List<VendorMobileModel> All { get; set; }
        public List<VendorMobileModel> Brand { get; set; }
        public List<VendorMobileModel> Designer { get; set; }
        public List<VendorMobileModel> SheEarns { get; set; }
    }
}