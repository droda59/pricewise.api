using Autofac;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Parsers;

namespace PriceAlerts.Common
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HtmlLoader>().As<IHtmlLoader>().SingleInstance();
            builder.RegisterType<ParserFactory>().As<IParserFactory>().SingleInstance();
            builder.RegisterType<AmazonParser>().As<IParser>().SingleInstance();
            builder.RegisterType<BestBuyParser>().As<IParser>().SingleInstance();

            builder.RegisterType<ProductFactory>().As<IProductFactory>().SingleInstance();
            
            builder.RegisterType<MonitoredProductRepository>().As<IProductRepository>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
        }
    }
}
