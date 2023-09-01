using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.Stripe
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //Submit PayPal Express Checkout button
            routes.MapRoute("Nop.Plugin.Payments.Stripe.PaymentInfo",
                 "Plugins/Stripe/PaymentInfo",
                 new { controller = "PaymentStripe", action = "PaymentInfo" , mobile = UrlParameter.Optional },
                 new[] { "Nop.Plugin.Payments.Stripe.Controllers" }
                 );
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
