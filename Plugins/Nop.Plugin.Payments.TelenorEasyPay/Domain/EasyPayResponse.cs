using Nop.Core;
using System;

namespace Nop.Plugin.Payments.TelenorEasyPay.Domain
{
    public class EasyPayResponse : BaseEntity
    {
        public virtual int OrderId { get; set; }
        public virtual string RawData { get; set; }
        public virtual bool IsOtcPayment { get; set; }
        public virtual DateTime UTCTime { get; set; }
    }
}
