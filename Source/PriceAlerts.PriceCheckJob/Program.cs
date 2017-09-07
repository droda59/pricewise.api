using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using PriceAlerts.Common.Database;
using PriceAlerts.PriceCheckJob.Jobs;

namespace PriceAlerts.PriceCheckJob
{
    public class PriceCheckJob
    {
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            MongoDBConfig.RegisterClassMaps();
            
			var builder = new ContainerBuilder();
            builder.RegisterModule(new Common.AutofacModule());
            builder.RegisterModule(new AutofacModule());

            var container = builder.Build();
            
            while (true)
            {
                var sw = Stopwatch.StartNew();

                var updatePricesJob = container.Resolve<UpdatePricesJob>();
                await updatePricesJob.UpdatePrices();

                sw.Stop();
                Console.WriteLine("Took " + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString(@"dd\.hh\:mm\:ss\.fff") + " to process prices.");
                Console.WriteLine();
                sw.Restart();

                var alertUsersJob = container.Resolve<AlertUsersJob>();
                await alertUsersJob.SendAlerts();

                sw.Stop();
                Console.WriteLine("Took " + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString(@"dd\.hh\:mm\:ss\.fff") + " to process alerts.");
                Console.WriteLine();

                Thread.Sleep(TimeSpan.FromHours(24));
            }
        }
    }
}