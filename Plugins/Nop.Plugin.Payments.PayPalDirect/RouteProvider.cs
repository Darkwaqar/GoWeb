using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.Awein
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.Awein.Webhook",
                 "Plugins/PaymentPayPalDirect/Webhook",
                 new { controller = "PaymentPayPalDirect", action = "WebhookEventsHandler" },
                 new[] { "Nop.Plugin.Payments.Awein.Controllers" }
            );
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
