using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProxyServer.Startup))]
namespace ProxyServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
