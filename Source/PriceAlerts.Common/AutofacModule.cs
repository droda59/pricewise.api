using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using PriceAlerts.Common.CommandHandlers;
using PriceAlerts.Common.Commands;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;
using Module = Autofac.Module;

namespace PriceAlerts.Common
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {   
            builder.RegisterType<HttpClient>().As<IRequestClient>().SingleInstance();
            builder.RegisterType<DocumentLoader>().As<IDocumentLoader>().SingleInstance();
            builder.RegisterType<HandlerFactory>().As<IHandlerFactory>().SingleInstance();

            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ISource)) && x.Name.EndsWith("Source"))
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ICommandHandler)) && x.Name.EndsWith("CommandHandler"))
                .As<ICommandHandler>()
                .SingleInstance()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions(new MethodGenerationHook()))
                .InterceptedBy(typeof(LoggerInterceptor));
            
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.GetInterfaces().Contains(typeof(ICommand)))
                .AsSelf()
                .SingleInstance()
                .EnableClassInterceptors(new ProxyGenerationOptions(new LoggingMethodGenerationHook()))
                .InterceptedBy(typeof(LoggerInterceptor));
            
            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .SingleInstance()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor));

//            builder.RegisterType<TraceLogger>().As<ILogger>().SingleInstance();
            builder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();
            builder.Register(c => new LoggerInterceptor(c.Resolve<ILogger>()));
        }
    }
}
