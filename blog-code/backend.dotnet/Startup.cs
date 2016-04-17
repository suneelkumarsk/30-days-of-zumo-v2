using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(backend.dotnet.Startup))]
namespace backend.dotnet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
