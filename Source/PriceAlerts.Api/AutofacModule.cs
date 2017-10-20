using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.Logging;
using PriceAlerts.Api.Factories;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAlertFactory>()
                .As<IUserAlertFactory>()
                .SingleInstance()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor));
            
            builder.RegisterType<ProductFactory>()
                .As<IProductFactory>()
                .SingleInstance()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor));
        }
    }
}
