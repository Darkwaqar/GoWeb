namespace Nop.Web.MServices.Models.Vendors
{
    public partial class ContactVendorModelAPI
    {
        public int vendorId { get; set; }
        public string vendorName { get; set; }
        public string email { get; set; }
        public string subject { get; set; }
        public string enquiry { get; set; }
        public string fullName { get; set; }
    }
}