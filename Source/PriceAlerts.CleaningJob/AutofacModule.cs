using Autofac;
using PriceAlerts.CleaningJob.Jobs;

namespace PriceAlerts.CleaningJob
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CleanUrlsJob>().AsSelf();
            builder.RegisterType<CleanHistoryJob>().AsSelf();
        }
    }
}