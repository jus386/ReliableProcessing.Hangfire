using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using DataAccess;
using DataAccess.EF;
using Hangfire;
using Owin;
using Processing;
using GlobalConfiguration = Hangfire.GlobalConfiguration;

namespace WebApp
{
	public partial class Startup
	{
	    public void ConfigureHangfire(IAppBuilder app)
	    {
            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireDb");

            var builder = new ContainerBuilder();
            builder.RegisterType<CountingProcessor>();
            builder.RegisterType<CheckProcessor>();
            builder.RegisterType<CalculationJobRepo>().As<ICalculationJobRepo>();

            // Handle EntityFramework DbContext instantiation through AutoFac
            builder.RegisterType<ProcessingDbContext>().InstancePerBackgroundJob();

            // Register your MVC controllers.
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}