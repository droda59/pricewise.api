using Autofac;

using PriceAlerts.Api.Factories;

namespace PriceAlerts.Api
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAlertFactory>().As<IUserAlertFactory>().SingleInstance();
        }
    }
}
