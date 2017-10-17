using Autofac;

namespace PriceAlerts.CleaningJob
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .AssignableTo<IJob>()
                .AsImplementedInterfaces();
        }
    }
}