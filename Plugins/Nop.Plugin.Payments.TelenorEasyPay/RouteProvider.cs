using Nop.Plugin.Payments.TelenorEasyPay.Infrastructure;
using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Payments.TelenorEasyPay
{
    class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payment.TelenorEasyPay.RedirectHandler",
                   "Plugins/PaymentEasyPay/RedirectHandler",
                    new { controller = "PaymentEasyPay", action = "RedirectHandler" },
                   new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }
              );

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayConfirmation",
              "Plugins/PaymentEasyPay/EasyPayConfirmation",
               new { controller = "PaymentEasyPay", action = "EasyPayConfirmation" },
              new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }
            );

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayReceipt",
              "Plugins/PaymentEasyPay/EasyPayReceipt",
               new { controller = "PaymentEasyPay", action = "EasyPayReceipt" },
              new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }
            );

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayIPNListener",
            "Plugins/PaymentEasyPay/EasyPayIPNListener",
             new { controller = "PaymentEasyPay", action = "EasyPayIPNListener" },
            new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }
            );

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.RedirectHandlerMobile",
                 "Plugins/PaymentEasyPay/RedirectHandlerMobile",
                  new { controller = "PaymentEasyPay", action = "RedirectHandlerMobile" },
                 new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }
            );

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayConfirmationMobile",
              "Plugins/PaymentEasyPay/EasyPayConfirmationMobile",
               new { controller = "PaymentEasyPay", action = "EasyPayConfirmationMobile" },
              new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }
            );

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayReceiptMobile",
              "Plugins/PaymentEasyPay/EasyPayReceiptMobile",
               new { controller = "PaymentEasyPay", action = "EasyPayReceiptMobile" },
              new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }
            );

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.Configure",
                 "Plugins/PaymentEasyPay/Configure",
                 new { controller = "PaymentEasyPay", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }).DataTokens.Add("area", "Admin");

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayPaymentStatusList",
                 "Plugins/PaymentEasyPay/EasyPayPaymentStatusList",
                 new { controller = "PaymentEasyPay", action = "EasyPayPaymentStatusList" },
                 new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" });

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayPaymentStatusCreate",
                 "Plugins/PaymentEasyPay/EasyPayPaymentStatusCreate",
                 new { controller = "PaymentEasyPay", action = "EasyPayPaymentStatusCreate" },
                 new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" });

            routes.MapRoute("Plugin.Payment.TelenorEasyPay.EasyPayPaymentStatusEdit",
               "Plugins/PaymentEasyPay/EasyPayPaymentStatusEdit/{Id}",
               new { controller = "PaymentEasyPay", action = "EasyPayPaymentStatusEdit", Id = 0 },
               new[] { "Nop.Plugin.Payments.TelenorEasyPay.Controllers" });


            ViewEngines.Engines.Insert(0, new EasyPayViewEngine());

        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
