using Autofac;
using Microsoft.Extensions.Logging;
using PriceAlerts.CleaningJob.Jobs;

namespace PriceAlerts.CleaningJob
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Except<CleanDuplicateProductsJob>()
                .Except<CleanHistoryJob>()
                .Except<CleanUrlsJob>()
                .AssignableTo<IJob>()
                .AsImplementedInterfaces();

            builder.Register<ILoggerFactory>(c => new LoggerFactory());
        }
    }
}