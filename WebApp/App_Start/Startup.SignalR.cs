using Owin;

namespace WebApp
{
	public partial class Startup
	{
	    public void ConfigureSignalR(IAppBuilder app)
	    {
            app.MapSignalR();
        }
    }
}