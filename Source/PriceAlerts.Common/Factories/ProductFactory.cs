using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Parsers;

namespace PriceAlerts.Common.Factories
{
    internal class ProductFactory : IProductFactory
    {
        private readonly IParserFactory _parserFactory;
        private readonly IProductRepository _productRepository;

        public ProductFactory(IParserFactory parserFactory, IProductRepository productRepository)
        {
            this._parserFactory = parserFactory;
            this._productRepository = productRepository;
        }

        public async Task<MonitoredProduct> CreateProduct(string uri)
        {
            var siteInfo = await this._parserFactory.CreateParser(uri).GetSiteInfo(uri);

            var monitoredProduct = new MonitoredProduct
            {
                Uri = siteInfo.Uri,
                Title = siteInfo.Title,
                ImageUrl = siteInfo.ImageUrl
            };

            monitoredProduct = await this._productRepository.InsertAsync(monitoredProduct);
            // TODO wtf is this with the ProductId?
            monitoredProduct.PriceHistory.Add(new PriceChange { Price = siteInfo.Price, ModifiedAt = DateTime.Now.ToUniversalTime(), ProductId = monitoredProduct.Id });

            monitoredProduct = await this._productRepository.UpdateAsync(monitoredProduct.Id, monitoredProduct);

            return monitoredProduct;
        }
    }
}