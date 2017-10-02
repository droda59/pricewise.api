using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    internal class ProductFactory : IProductFactory
    {
        private readonly IHandlerFactory _handlerFactory;
        private readonly IProductRepository _productRepository;

        public ProductFactory(IHandlerFactory handlerFactory, IProductRepository productRepository)
        {
            this._handlerFactory = handlerFactory;
            this._productRepository = productRepository;
        }

        public async Task<MonitoredProduct> CreateProduct(Uri url)
        {
            var handler = this._handlerFactory.CreateHandler(url);
            var siteInfo = await handler.HandleGetInfo(url);
            
            if (siteInfo == null)
            {
                throw new ParseException("PriceWise was unable to get the product information.", url);
            }

            var monitoredProduct = new MonitoredProduct
            {
                ProductIdentifier = siteInfo.ProductIdentifier, 
                Uri = siteInfo.Uri,
                Title = siteInfo.Title,
                ImageUrl = siteInfo.ImageUrl
            };

            monitoredProduct.PriceHistory.Add(new PriceChange { Price = siteInfo.Price, ModifiedAt = DateTime.UtcNow });
            monitoredProduct = await this._productRepository.InsertAsync(monitoredProduct);

            return monitoredProduct;
        }
    }
}