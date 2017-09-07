using System;

using Microsoft.Extensions.DependencyInjection;

using Autofac;
using Autofac.Extensions.DependencyInjection;

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
            
            var container = builder.Build();

            return container.Resolve<IServiceProvider>();
        }
    }
}