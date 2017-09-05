using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Parsers;

namespace PriceAlerts.PriceCheckJob.Jobs
{
    internal class UpdatePricesJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IParserFactory _parserFactory;

        public UpdatePricesJob(IProductRepository productRepository, IParserFactory parserFactory)
        {
            this._productRepository = productRepository;
            this._parserFactory = parserFactory;
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
                    var siteInfo = await this._parserFactory.CreateParser(productUri).GetSiteInfo(productUri);

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