using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InteriorDesign.Startup))]
namespace InteriorDesign
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
