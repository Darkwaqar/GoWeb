

namespace Nop.Core.Domain.Vendors
{
    public partial class VendorPaymentInfo : BaseEntity
    {
        public int VendorId { get; set; }

        public string MobileNumber { get; set; }

        public string CNIC { get; set; }

        public string BankDetails { get; set; }

        public int CheckCopyImageId { get; set; }

        public string BankName { get; set; }

        public string Branch { get; set; }

        public string AccountNumber { get; set; }
    }
}
