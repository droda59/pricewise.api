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
                try 
                {
                    Console.WriteLine("Starting product " + product.Id);

                    var productUri = new Uri(product.Uri);
                    var siteInfo = await this._parserFactory.CreateParser(productUri).GetSiteInfo(productUri);
                    
                    // This is made so the current products that have no identifier can try to fetch one
                    // TODO Remove when all products have an identifier
                    if (string.IsNullOrWhiteSpace(product.ProductIdentifier))
                    {
                        product.ProductIdentifier = siteInfo.ProductIdentifier;
                    }

                    product.PriceHistory.Add(
                        new PriceChange
                        {
                            Price = siteInfo.Price,
                            ModifiedAt = DateTime.Now.ToUniversalTime()
                        });

                    await this._productRepository.UpdateAsync(product.Id, product);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Found error on product " + product.Id + ": " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Finished product " + product.Id);
                }
            }));
        }
    }
}