using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.TelenorEasyPay.Data
{
    public class EasyPayPaymentStatusUrlMap : EntityTypeConfiguration<EasyPayPaymentStatusUrl>
    {
        public EasyPayPaymentStatusUrlMap()
        {
            ToTable("EasyPayPaymentStatusUrl");

            //Map the primary key
            HasKey(m => m.Id);
            //Map the additional properties
            Property(m => m.OrderId).IsRequired();
            Property(m => m.Url).IsRequired().HasMaxLength(512);
            Property(m => m.UTCTime).IsRequired();
        }
    }
}
