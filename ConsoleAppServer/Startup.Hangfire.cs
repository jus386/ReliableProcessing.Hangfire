using Autofac;
using DataAccess;
using DataAccess.EF;
using Hangfire;
using Owin;
using Processing;
using GlobalConfiguration = Hangfire.GlobalConfiguration;

namespace ConsoleAppServer
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

            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());

            app.UseHangfireDashboard();
        }
    }
}