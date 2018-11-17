using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(farmarproject2.Startup))]
namespace farmarproject2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
