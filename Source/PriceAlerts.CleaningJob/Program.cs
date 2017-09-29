using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using PriceAlerts.Common.Database;
using PriceAlerts.CleaningJob.Jobs;

namespace PriceAlerts.CleaningJob
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

                var cleanUrlsJob = container.Resolve<CleanUrlsJob>();
                await cleanUrlsJob.ExecuteJob();

                sw.Stop();
                Console.WriteLine("Took " + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString(@"dd\.hh\:mm\:ss\.fff") + " to clean urls.");
                Console.WriteLine();

                var cleanHistoryJob = container.Resolve<CleanHistoryJob>();
                await cleanHistoryJob.ExecuteJob();

                sw.Stop();
                Console.WriteLine("Took " + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString(@"dd\.hh\:mm\:ss\.fff") + " to clean history.");
                Console.WriteLine();

                Thread.Sleep(TimeSpan.FromDays(7));
            }
        }
    }
}