using System.Linq;
using System.Reflection;

using Autofac;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClient>().As<IRequestClient>().SingleInstance();
            builder.RegisterType<DocumentLoader>().As<IDocumentLoader>().SingleInstance();

            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ISource)) && x.Name.EndsWith("Source"))
                .AsSelf()
                .SingleInstance();

            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(IParser)) && x.Name.EndsWith("Parser"))
                .Except<AmazonHtmlParser>()
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
