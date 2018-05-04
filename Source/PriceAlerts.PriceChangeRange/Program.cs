using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using PriceAlerts.Common.Database;

namespace PriceAlerts.PriceChangeRange
{
    public class PriceChangeRange
    {
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            MongoDBConfig.RegisterClassMaps();
            
            var builder = new ContainerBuilder();
            builder.Register<ILoggerFactory>(c => new LoggerFactory());
            builder.RegisterModule(new Common.AutofacModule());

            var container = builder.Build();
            var productRepository = container.Resolve<IProductRepository>();
            
            var changes = new List<ChangeInfo>();

            
            var sw = Stopwatch.StartNew();
            
            var allProducts = await productRepository.GetAllAsync();
            foreach (var product in allProducts)
            {
                var orderedPriceHistory = product.PriceHistory.OrderBy(x => x.ModifiedAt).ToList();
                for (var i = 0; i < orderedPriceHistory.Count - 1; i++)
                {
                    if (orderedPriceHistory[i].Price != orderedPriceHistory[i + 1].Price)
                    {
                        changes.Add(new ChangeInfo { Domain = new Uri(product.Uri).Authority, DateTime = orderedPriceHistory[i + 1].ModifiedAt });
                    }
                }
            }
            
            sw.Stop();
            Console.WriteLine($"Took {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds):dd\\.hh\\:mm\\:ss\\.fff} to process price changes.");
            Console.WriteLine();

            var changeDick = changes.ToLookup(x => x.Domain);
            foreach (var domain in changeDick)
            {
                Console.WriteLine("Changes for " + domain.Key + ": ");

                /*
                var domainChanges = domain.ToList().OrderBy(x => x.DateTime);
                foreach (var change in domainChanges)
                {
                    Console.WriteLine($"A change happened on {change.DateTime.Date.ToShortDateString()} which was a {change.DateTime.DayOfWeek}");
                }
                
                Console.WriteLine();
                */

                var dayChanges = domain.ToLookup(x => x.DateTime.DayOfWeek);
                foreach (var dayChange in dayChanges)
                {
                    Console.WriteLine($"{dayChange.Count()} changes happened on {dayChange.Key}");
                }
                
                Console.WriteLine();
            }
        }

        private class ChangeInfo
        {
            public string Domain { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}