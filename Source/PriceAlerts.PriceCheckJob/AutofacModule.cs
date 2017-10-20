using Autofac;
using Microsoft.Extensions.Logging;
using PriceAlerts.PriceCheckJob.Emails;
using PriceAlerts.PriceCheckJob.Jobs;

namespace PriceAlerts.PriceCheckJob
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpdatePricesJob>().AsSelf();
            builder.RegisterType<AlertUsersJob>().AsSelf();

            builder.RegisterType<EmailSender>().As<IEmailSender>().SingleInstance();

            builder.Register<ILoggerFactory>(c => new LoggerFactory());
        }
    }
}