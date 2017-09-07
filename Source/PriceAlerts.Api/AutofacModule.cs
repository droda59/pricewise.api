using Autofac;

using PriceAlerts.Api.Factories;

namespace PriceAlerts.Api
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAlertFactory>().As<IUserAlertFactory>().SingleInstance();
            builder.RegisterType<ProductFactory>().As<IProductFactory>().SingleInstance();
            builder.RegisterType<SearcherFactory>().As<ISearcherFactory>().SingleInstance();
        }
    }
}
