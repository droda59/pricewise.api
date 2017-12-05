using Autofac;
using Autofac.Extras.DynamicProxy;
using PriceAlerts.Api.Factories;
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

//            builder.RegisterType<UserAlertFactory>()
//                .As<IUserAlertFactory>()
//                .SingleInstance()
//                .EnableInterfaceInterceptors()
//                .InterceptedBy(typeof(LoggerInterceptor));
//            
//            builder.RegisterType<AlertListFactory>()
//                .As<IAlertListFactory>()
//                .SingleInstance()
//                .EnableInterfaceInterceptors()
//                .InterceptedBy(typeof(LoggerInterceptor));
//            
//            builder.RegisterType<ProductFactory>()
//                .As<IProductFactory>()
//                .SingleInstance()
//                .EnableInterfaceInterceptors()
//                .InterceptedBy(typeof(LoggerInterceptor));
        }
    }
}
