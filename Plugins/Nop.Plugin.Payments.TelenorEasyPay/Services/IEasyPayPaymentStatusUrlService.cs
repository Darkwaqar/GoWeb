using Nop.Plugin.Payments.TelenorEasyPay.Domain;
namespace Nop.Plugin.Payments.TelenorEasyPay.Services
{
    public interface IEasyPayPaymentStatusUrlService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Log(EasyPayPaymentStatusUrl record);

        EasyPayPaymentStatusUrl GetEasyPayPaymentStatusUrlByOrderId(int orderId);
    }
}
