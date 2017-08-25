using System.Linq;
using System.Reflection;

using Autofac;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.LinkManipulators;
using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Api.SourceHandlers;

namespace PriceAlerts.Api
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAlertFactory>().As<IUserAlertFactory>().SingleInstance();
            builder.RegisterType<ProductFactory>().As<IProductFactory>().SingleInstance();
            builder.RegisterType<HandlerFactory>().As<IHandlerFactory>().SingleInstance();
            builder.RegisterType<SearcherFactory>().As<ISearcherFactory>().SingleInstance();
            
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(IHandler)) && x.Name.EndsWith("Handler"))
                .As<IHandler>()
                .SingleInstance();
            
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ICleaner)) && x.Name.EndsWith("Cleaner"))
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ILinkManipulator)) && x.Name.EndsWith("LinkManipulator"))
                .AsSelf()
                .SingleInstance();
        }
    }
}
