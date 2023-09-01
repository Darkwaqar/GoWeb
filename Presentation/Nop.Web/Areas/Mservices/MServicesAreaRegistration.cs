using System.Web.Mvc;

namespace Nop.Web.Areas.MServices
{
    public class MServicesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "MServices"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MServices_default",
                "MServices/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}