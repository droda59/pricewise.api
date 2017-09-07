using System.Linq;
using Autofac;

using PriceAlerts.Api.Factories;
using PriceAlerts.Common.Commands.Searchers;

namespace PriceAlerts.Api
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAlertFactory>().As<IUserAlertFactory>().SingleInstance();
            builder.RegisterType<ProductFactory>().As<IProductFactory>().SingleInstance();
            
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ISearcher)) && x.Name.EndsWith("Searcher"))
                .AsSelf()
                .As<ISearcher>()
                .SingleInstance();
        }
    }
}
