using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using DataAccess;
using DataAccess.EF;
using Processing;

namespace WebApp
{
    public class Bootstraper
    {
        public static void Init()
        {
            InitMvc();
        }

        private static void InitMvc()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<CalculationJobRepo>().As<ICalculationJobRepo>();
            builder.RegisterType<CountingProcessor>();
            builder.RegisterType<CheckProcessor>();

            // Handle EntityFramework DbContext instantiation through AutoFac
            builder.RegisterType<ProcessingDbContext>().
                As<ProcessingDbContext>().InstancePerRequest();

            // Register your MVC controllers.
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}