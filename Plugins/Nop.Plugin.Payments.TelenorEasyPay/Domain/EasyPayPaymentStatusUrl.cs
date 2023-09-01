using Nop.Core;
using System;

namespace Nop.Plugin.Payments.TelenorEasyPay.Domain
{
    public class EasyPayPaymentStatusUrl : BaseEntity
    {
        public virtual int OrderId { get; set; }
        public virtual string Url { get; set; }
        public virtual DateTime UTCTime { get; set; }

    }

}
