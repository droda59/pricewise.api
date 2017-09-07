using System.Linq;
using System.Reflection;

using Autofac;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.LinkManipulators;
using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Searchers;
using PriceAlerts.Common.SourceHandlers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClient>().As<IRequestClient>().SingleInstance();
            builder.RegisterType<DocumentLoader>().As<IDocumentLoader>().SingleInstance();
            builder.RegisterType<HandlerFactory>().As<IHandlerFactory>().SingleInstance();
            
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

            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ISource)) && x.Name.EndsWith("Source"))
                .AsSelf()
                .SingleInstance();

            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(IParser)) && x.Name.EndsWith("Parser"))
                .AsSelf()
                .As<IParser>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ISearcher)) && x.Name.EndsWith("Searcher"))
                .AsSelf()
                .As<ISearcher>()
                .SingleInstance();

            builder.RegisterType<MonitoredProductRepository>().As<IProductRepository>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<AlertRepository>().As<IAlertRepository>().SingleInstance();
        }
    }
}
