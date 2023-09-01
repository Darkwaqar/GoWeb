using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    class VendorPaymentInfoMap : NopEntityTypeConfiguration<VendorPaymentInfo>
    {
        public VendorPaymentInfoMap()
        {
            this.ToTable("VendorPaymentInfo");
            this.HasKey(v => v.Id);
            this.Property(v => v.VendorId);
            this.Property(v => v.AccountNumber);
            this.Property(v => v.BankDetails);
            this.Property(v => v.BankName);
            this.Property(v => v.Branch);
            this.Property(v => v.CheckCopyImageId);
            this.Property(v => v.CNIC);
            this.Property(v => v.MobileNumber);
        }
    }
}
