using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RPGSite.Startup))]
namespace RPGSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
