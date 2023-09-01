using Nop.Web.Framework.Themes;

namespace Nop.Plugin.Payments.TelenorEasyPay.Infrastructure
{
    public class EasyPayViewEngine : ThemeableRazorViewEngine
    {
        public EasyPayViewEngine()
        {
            ViewLocationFormats = new[] { "~/Plugins/Payments.TelenorEasyPay/Views/PaymentEasyPay/{0}.cshtml" };
            PartialViewLocationFormats = new[] { "~/Plugins/Payments.TelenorEasyPay/Views/PaymentEasyPay/{0}.cshtml" };
        }

    }

}
