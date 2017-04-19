using Autofac;

using PriceAlerts.PriceCheckJob.Jobs;

namespace PriceAlerts.PriceCheckJob
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpdatePricesJob>().AsSelf();
            builder.RegisterType<AlertUsersJob>().AsSelf();
        }
    }
}