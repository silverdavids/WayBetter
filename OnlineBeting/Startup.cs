using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineBeting.Startup))]
namespace OnlineBeting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
