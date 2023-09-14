using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Assignment_V2.Startup))]
namespace Assignment_V2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
