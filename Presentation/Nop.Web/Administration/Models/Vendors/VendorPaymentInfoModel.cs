using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Admin.Models.Vendors
{
    public class VendorPaymentInfoModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Vendors.Fields.VendorId")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.MobileNumber")]
        public string  MobileNumber { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CNIC")]
        public string CNIC { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.BankDetails")]
        public string BankDetails { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CheckCopyImageId")]
        [UIHint("Picture")]
        public int CheckCopyImageId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.BankName")]
        public string BankName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Branch")]
        public string Branch { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.AccountNumber")]
        public string AccountNumber { get; set; }

    }
}