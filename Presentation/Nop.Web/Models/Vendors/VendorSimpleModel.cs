using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Vendors
{
    public class VendorSimpleModel : BaseNopModel
    {
        public int Id { get;  set; }
        public string Name { get;  set; }
        public string PictureUrl { get;  set; }
    }
}