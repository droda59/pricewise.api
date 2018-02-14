using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Models;

namespace PriceAlerts.PriceCheckJob.Jobs
{
    internal class UpdatePricesJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IPriceCheckRunStatisticsRepository _statisticsRepository;
        private readonly IHandlerFactory _handlerFactory;

        public UpdatePricesJob(IProductRepository productRepository, IPriceCheckRunStatisticsRepository statisticsRepository, IHandlerFactory handlerFactory)
        {
            this._productRepository = productRepository;
            this._statisticsRepository = statisticsRepository;
            this._handlerFactory = handlerFactory;
        }
        
        public async Task UpdatePrices()
        {
            var allProducts = await this._productRepository.GetAllAsync();

            var statistics = new Dictionary<Uri, PriceCheckRunDomainStatistics>();

            foreach (var product in allProducts)
            {
                var productUri = new Uri(product.Uri);
                var handler = this._handlerFactory.CreateHandler(productUri);
                if (!statistics.ContainsKey(handler.Domain))
                {
                    statistics.Add(handler.Domain, new PriceCheckRunDomainStatistics { Domain = handler.Domain.ToString() });
                }

                try
                {
                    var siteInfo = await handler.HandleGetInfo(productUri);
                    if (siteInfo != null)
                    {
                        var lastEntry = product.PriceHistory.LastOf(x => x.ModifiedAt);
                        
                        var newPrice = siteInfo.Price;
                        var lastPrice = lastEntry.Price;

                        var currentDate = DateTime.UtcNow.Date;
                        var lastDate = lastEntry.ModifiedAt.Date;
                        
                        if (newPrice == lastPrice && currentDate == lastDate)
                        {
                            statistics[handler.Domain].Unhandled++;
                        }
                        else
                        {
                            product.PriceHistory.Add(
                                new PriceChange
                                {
                                    Price = newPrice,
                                    ModifiedAt = DateTime.UtcNow
                                });
                         
                            await this._productRepository.UpdateAsync(product.Id, product);   

                            statistics[handler.Domain].Successes++;
                        }

                    }
                }
                catch (Exception)
                {
                    statistics[handler.Domain].Errors++;
                }
            }
            
            Console.WriteLine($"Found {statistics.Values.Select(x => x.Successes).Sum()} successes.");
            Console.WriteLine($"Found {statistics.Values.Select(x => x.Errors).Sum()} errors.");
            Console.WriteLine($"Found {statistics.Values.Select(x => x.Unhandled).Sum()} unhandled.");

            var priceCheckRunStatistics = await this._statisticsRepository.InsertAsync(statistics.Values);
            Console.WriteLine($"Saved run as {priceCheckRunStatistics.Id}.");

//            foreach (var domain in statistics)
//            {
//                Console.WriteLine($"Found {domain.Value.Successes} success, {domain.Value.Errors} errors and {domain.Value.Unhandled} unhandled on {domain.Key}");
//            }
        }
    }
}