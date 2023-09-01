using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Payments.TelenorEasyPay.Data
{
    public class EasyPayResponseMap : EntityTypeConfiguration<EasyPayResponse>
    {
        public EasyPayResponseMap()
        {
            ToTable("EasyPayResponse");
            //Map the primary key
            HasKey(m => m.Id);
            //Map the additional properties
            Property(m => m.OrderId).IsRequired();
            Property(m => m.RawData).IsRequired().HasMaxLength(512);
            Property(m => m.IsOtcPayment).IsRequired();
            Property(m => m.UTCTime).IsRequired();
        }
    }
}
