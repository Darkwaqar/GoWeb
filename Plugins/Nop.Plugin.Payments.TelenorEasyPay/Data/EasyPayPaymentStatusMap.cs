using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using System.Data.Entity.ModelConfiguration;


namespace Nop.Plugin.Payments.TelenorEasyPay.Data
{
    public class EasyPayPaymentStatusMap : EntityTypeConfiguration<EasyPayPaymentStatus>
    {

        public EasyPayPaymentStatusMap()
        {
            ToTable("EasyPayPaymentStatus");

            //Map the primary key
            HasKey(m => m.Id);
            //Map the additional properties
            Property(m => m.AccountNumber).IsOptional().HasMaxLength(35);
            Property(m => m.Description).IsOptional().HasMaxLength(100);
            Property(m => m.Msisdn).IsOptional().HasMaxLength(35);
            Property(m => m.OrderDatetime).IsOptional().HasMaxLength(35);
            Property(m => m.OrderId).IsRequired();
            Property(m => m.PaidDatetime).IsOptional().HasMaxLength(35);
            Property(m => m.PaymentMethod).IsOptional().HasMaxLength(5);
            Property(m => m.PaymentToken).IsOptional().HasMaxLength(35);
            Property(m => m.ResponseCode).IsOptional().HasMaxLength(10);
            Property(m => m.StoreId).IsOptional();
            Property(m => m.StoreName).IsOptional().HasMaxLength(100);
            Property(m => m.TokenExpiryDatetime).IsOptional().HasMaxLength(35);
            Property(m => m.TransactionAmount).IsOptional().HasPrecision(18, 4);
            Property(m => m.TransactionId).IsOptional().HasMaxLength(35);
            Property(m => m.TransactionStatus).IsOptional().HasMaxLength(35);
            Property(m => m.UTCTime).IsRequired();
            Property(m => m.RawData).IsRequired().HasMaxLength(512);
        }

    }
}
