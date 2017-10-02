using System.Reflection;
using Autofac;

namespace PriceAlerts.CleaningJob
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess)
                .AssignableTo<IJob>()
                .AsImplementedInterfaces();
        }
    }
}