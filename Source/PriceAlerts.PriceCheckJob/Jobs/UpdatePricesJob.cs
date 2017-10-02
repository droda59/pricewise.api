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
        private readonly IHandlerFactory _handlerFactory;

        public UpdatePricesJob(IProductRepository productRepository, IHandlerFactory handlerFactory)
        {
            this._productRepository = productRepository;
            this._handlerFactory = handlerFactory;
        }
        
        public async Task UpdatePrices()
        {
            var allProducts = await this._productRepository.GetAllAsync();

            var errorDick = new Dictionary<string, ParseInfo>();

            foreach (var product in allProducts)
            {
                var productUri = new Uri(product.Uri);
                if (!errorDick.ContainsKey(productUri.Authority))
                {
                    errorDick.Add(productUri.Authority, new ParseInfo());
                }

                try
                {
                    var handler = this._handlerFactory.CreateHandler(productUri);
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
                            errorDick[productUri.Authority].Unhandled++;
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

                            errorDick[productUri.Authority].Success++;
                        }

                    }
                }
                catch (Exception)
                {
                    errorDick[productUri.Authority].Errors++;
                }
            }
            
            Console.WriteLine($"Found {errorDick.Values.Select(x => x.Success).Sum()} successes.");
            Console.WriteLine($"Found {errorDick.Values.Select(x => x.Errors).Sum()} errors.");
            Console.WriteLine($"Found {errorDick.Values.Select(x => x.Unhandled).Sum()} unhandled.");

            foreach (var domain in errorDick)
            {
                Console.WriteLine($"Found {domain.Value.Success} success, {domain.Value.Errors} errors and {domain.Value.Unhandled} unhandled on {domain.Key}");
            }
        }

        private class ParseInfo
        {
            public int Errors { get; set; }
            public int Success { get; set; }
            public int Unhandled { get; set; }
        }
    }
}