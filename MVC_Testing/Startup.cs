using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_Testing.Startup))]
namespace MVC_Testing
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
