using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using PriceAlerts.Api.Infrastructure;
using PriceAlerts.Common.Database;

namespace PriceAlerts.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
                
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors(options => {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                })
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    var settings = options.SerializerSettings;
                    
                    settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.Formatting = Formatting.Indented;
                });

            services.AddAuthorization(options => 
            {
                options.AddPolicy("LocalOnly", policy => policy.Requirements.Add(new LocalAuthorizationOnlyRequirement()));
            });

            MongoDBConfig.RegisterClassMaps();
            
            return IocConfig.RegisterComponents(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("CorsPolicy");
            
            var options = new JwtBearerOptions
            {
                Audience = this.Configuration["Auth0:ApiIdentifier"],
                Authority = $"https://{this.Configuration["Auth0:Domain"]}/"
            };
            app.UseJwtBearerAuthentication(options);

            app.UseMvc();
        }
    }
}
