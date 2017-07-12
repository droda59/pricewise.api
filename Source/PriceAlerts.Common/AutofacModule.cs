using System.Linq;
using System.Reflection;

using Autofac;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Common
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HtmlLoader>().As<IHtmlLoader>().SingleInstance();
            
            builder.RegisterType<ParserFactory>().As<IParserFactory>().SingleInstance();
            builder.RegisterAssemblyTypes(typeof(IParser).GetTypeInfo().Assembly)
                .Where(x => x.GetInterfaces().Contains(typeof(IParser)) && x.Name.EndsWith("Parser"))
                .As<IParser>()
                .SingleInstance();

            builder.RegisterType<SearcherFactory>().As<ISearcherFactory>().SingleInstance();
            builder.RegisterAssemblyTypes(typeof(ISearcher).GetTypeInfo().Assembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ISearcher)) && x.Name.EndsWith("Searcher"))
                .As<ISearcher>()
                .SingleInstance();

            builder.RegisterType<ProductFactory>().As<IProductFactory>().SingleInstance();
            
            builder.RegisterType<MonitoredProductRepository>().As<IProductRepository>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<AlertRepository>().As<IAlertRepository>().SingleInstance();
        }
    }
}
