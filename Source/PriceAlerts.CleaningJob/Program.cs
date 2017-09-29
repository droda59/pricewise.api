using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using PriceAlerts.Common.Database;

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
                var jobs = container.Resolve<IEnumerable<IJob>>();
                foreach (var job in jobs)
                {
                    Console.WriteLine($"Starting {job.GetType().Name} job.");
                    var sw = Stopwatch.StartNew();

                    await job.ExecuteJob();

                    sw.Stop();
                    Console.WriteLine($"Took {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds):dd\\.hh\\:mm\\:ss\\.fff} to execute {job.GetType().Name} job.");
                    Console.WriteLine();
                }
                
                Thread.Sleep(TimeSpan.FromDays(7));
            }
        }
    }
}