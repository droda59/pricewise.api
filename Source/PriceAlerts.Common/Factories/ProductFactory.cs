using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Models;
using PriceAlerts.Common.Parsers;

namespace PriceAlerts.Common.Factories
{
    internal class ProductFactory : IProductFactory
    {
        private readonly IParser _parser;

        public ProductFactory(IParser parser)
        {
            this._parser = parser;
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

            monitoredProduct.PriceHistory.Add(new PriceChange { Price = siteInfo.Price, ModifiedAt = DateTime.Now.ToUniversalTime() });

            return monitoredProduct;
        }
    }
}