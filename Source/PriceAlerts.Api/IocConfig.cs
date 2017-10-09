using System;
using Microsoft.Extensions.DependencyInjection;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api
{
    public static class IocConfig
    {
        public static IServiceProvider RegisterComponents(IServiceCollection services)
        {
			var builder = new ContainerBuilder();

            builder.RegisterModule(new Api.AutofacModule());
            builder.RegisterModule(new Common.AutofacModule());

            builder.Populate(services);

            // It is really important that this code gets executed AFTER the Populate(services) line
            builder.RegisterAssemblyTypes(typeof(IocConfig).Assembly)
                .Where(x => x.IsSubclassOf(typeof(Controller)) && x.Name.EndsWith("Controller"))
                .EnableClassInterceptors(new ProxyGenerationOptions(new LoggingMethodGenerationHook()))
                .InterceptedBy(typeof(LoggerInterceptor));
            
            var container = builder.Build();

            return container.Resolve<IServiceProvider>();
        }
    }
}