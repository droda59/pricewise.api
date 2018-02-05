using Autofac;
using Autofac.Extras.DynamicProxy;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.Name.EndsWith("Factory"))
                .AsImplementedInterfaces()
                .SingleInstance()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor));
        }
    }
}
