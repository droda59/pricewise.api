using System.Reflection;
using Autofac;

using PriceAlerts.Api.Factories;
using PriceAlerts.Common.Commands.Searchers;
using PriceAlerts.Common.Commands.Searchers.Sources;
using Module = Autofac.Module;

namespace PriceAlerts.Api
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAlertFactory>().As<IUserAlertFactory>().SingleInstance();
            builder.RegisterType<ProductFactory>().As<IProductFactory>().SingleInstance();
            
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(ISearcher)))
                .AssignableTo<ISearcher>()
                .Except<AmazonSearcher>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
