using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Parsers;

namespace PriceAlerts.Common.Factories
{
    internal class ProductFactory : IProductFactory
    {
        private readonly IParser _parser;
        private readonly IProductRepository _productRepository;

        public ProductFactory(IParser parser, IProductRepository productRepository)
        {
            this._parser = parser;
            this._productRepository = productRepository;
        }

        public async Task<MonitoredProduct> CreateProduct(string uri)
        {
            var siteInfo = await this._parser.GetSiteInfo(uri);

            var monitoredProduct = new MonitoredProduct
            {
                Uri = siteInfo.Uri,
                Title = siteInfo.Title,
                ImageUrl = siteInfo.ImageUrl
            };

            await this._productRepository.InsertAsync(monitoredProduct);
            monitoredProduct = await this._productRepository.GetByUrlAsync(uri);

            monitoredProduct.PriceHistory.Add(new PriceChange { Price = siteInfo.Price, ModifiedAt = DateTime.Now.ToUniversalTime(), ProductId = monitoredProduct.Id });

            await this._productRepository.UpdateAsync(monitoredProduct.Id, monitoredProduct);

            return monitoredProduct;
        }
    }
}