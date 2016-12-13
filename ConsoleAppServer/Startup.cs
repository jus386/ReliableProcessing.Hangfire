using Owin;

namespace ConsoleAppServer
{
	public partial class Startup
	{
        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage();
#endif
            app.UseWelcomePage("/");

            ConfigureHangfire(app);
        }
    }
}