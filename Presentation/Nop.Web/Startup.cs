using Owin;

namespace Nop.Web
{
    public partial class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}