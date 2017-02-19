using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(IsiiSports.Backend.Startup))]

namespace IsiiSports.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}