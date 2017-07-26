using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Cleaners;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Parsers;

namespace PriceAlerts.Common.Factories
{
    internal class ProductFactory : IProductFactory
    {
        private readonly IParserFactory _parserFactory;
        private readonly IProductRepository _productRepository;
        private readonly ICleanerFactory _cleanerFactory;

        public ProductFactory(IParserFactory parserFactory, IProductRepository productRepository, ICleanerFactory cleanerFactory)
        {
            this._parserFactory = parserFactory;
            this._productRepository = productRepository;
            this._cleanerFactory = cleanerFactory;
        }

        public async Task<MonitoredProduct> CreateProduct(Uri uri)
        {
            var siteInfo = await this._parserFactory.CreateParser(uri).GetSiteInfo(uri);
            var siteInfoUrl = new Uri(siteInfo.Uri);
            var cleanUrl = this._cleanerFactory.CreateCleaner(siteInfoUrl).CleanUrl(siteInfoUrl);

            var monitoredProduct = new MonitoredProduct
            {
                ProductIdentifier = siteInfo.ProductIdentifier, 
                Uri = cleanUrl.AbsoluteUri,
                Title = siteInfo.Title,
                ImageUrl = siteInfo.ImageUrl
            };

            monitoredProduct.PriceHistory.Add(new PriceChange { Price = siteInfo.Price, ModifiedAt = DateTime.Now.ToUniversalTime() });
            monitoredProduct = await this._productRepository.InsertAsync(monitoredProduct);

            return monitoredProduct;
        }
    }
}