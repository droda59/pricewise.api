using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common;
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
                try
                {
                    var handler = this._handlerFactory.CreateHandler(productUri);
                    var siteInfo = await handler.HandleGetInfo(productUri);

                    var lastPrice = product.PriceHistory.LastOf(x => x.ModifiedAt).Price;
                    var newPrice = siteInfo.Price;

                    if (lastPrice != newPrice)
                    {
                        Console.WriteLine($"Price changed from {lastPrice} to {newPrice} for item {product.Uri}");
                    }
                    
                    product.PriceHistory.Add(
                        new PriceChange
                        {
                            Price = siteInfo.Price,
                            ModifiedAt = DateTime.Now.ToUniversalTime()
                        });

                    await this._productRepository.UpdateAsync(product.Id, product);

                    if (!errorDick.ContainsKey(productUri.Authority))
                        errorDick.Add(productUri.Authority, new ParseInfo());

                    errorDick[productUri.Authority].Success++;
                }
                catch (Exception)
                {
                    if (!errorDick.ContainsKey(productUri.Authority))
                        errorDick.Add(productUri.Authority, new ParseInfo());

                    errorDick[productUri.Authority].Errors++;
                }
            }
            
            Console.WriteLine("Found " + errorDick.Values.Select(x => x.Success).Sum() + " successes.");
            Console.WriteLine("Found " + errorDick.Values.Select(x => x.Errors).Sum() + " errors.");
            
            foreach (var domain in errorDick)
                Console.WriteLine("Found " + domain.Value.Errors + " errors and " + domain.Value.Success + " success on " + domain.Key);
        }

        private class ParseInfo
        {
            public int Errors { get; set; }
            public int Success { get; set; }
        }
    }
}