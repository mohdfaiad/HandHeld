using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PI_MVC.Startup))]
namespace PI_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
