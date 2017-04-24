using System;
using System.Linq;
using System.Threading.Tasks;

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

            await Task.WhenAll(allProducts.Select(async product => 
            {
                Console.WriteLine("Starting product " + product.Id);

                var siteInfo = await this._parserFactory.CreateParser(product.Uri).GetSiteInfo(product.Uri);

                product.PriceHistory.Add(
                    new PriceChange
                    {
                        Price = siteInfo.Price,
                        ModifiedAt = DateTime.Now,
                        ProductId = product.Id
                    });

                await this._productRepository.UpdateAsync(product.Id, product);
                
                Console.WriteLine("Finished product " + product.Id);
            }));
        }
    }
}