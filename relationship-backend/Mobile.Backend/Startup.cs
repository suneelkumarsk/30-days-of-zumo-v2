using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Mobile.Backend.Startup))]

namespace Mobile.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}