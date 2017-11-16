using Microsoft.Owin;
using Owin;
using Peiyong.WebPc;


[assembly: OwinStartup(typeof(Startup))]

namespace Peiyong.WebPc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
