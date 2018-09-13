using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Extensions;
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
            var cleanUrl = handler.HandleCleanUrl(url);
            
            var monitoredProduct = await this._productRepository.GetByUrlAsync(cleanUrl.AbsoluteUri);
            if (monitoredProduct == null)
            {
                var siteInfo = await handler.HandleGetInfo(cleanUrl);

                if (siteInfo == null)
                {
                    throw new ParseException("PriceWise was unable to get the product information.", cleanUrl);
                }

                monitoredProduct = new MonitoredProduct
                {
                    ProductIdentifier = siteInfo.ProductIdentifier,
                    Uri = siteInfo.Uri,
                    Title = siteInfo.Title,
                    ImageUrl = siteInfo.ImageUrl
                };

                monitoredProduct.PriceHistory.Add(new PriceChange {Price = siteInfo.Price, ModifiedAt = DateTime.UtcNow});
                monitoredProduct = await this._productRepository.InsertAsync(monitoredProduct);
            }

            return monitoredProduct;
        }

        public async Task<MonitoredProduct> CreateUpdatedProduct(Uri url)
        {
            var handler = this._handlerFactory.CreateHandler(url);
            var cleanUrl = handler.HandleCleanUrl(url);
            
            var siteInfo = await handler.HandleGetInfo(cleanUrl);
            if (siteInfo == null)
            {
                throw new ParseException("PriceWise was unable to get the product information.", cleanUrl);
            }
            
            var monitoredProduct = await this._productRepository.GetByUrlAsync(cleanUrl.AbsoluteUri);
            if (monitoredProduct == null)
            {
                monitoredProduct = new MonitoredProduct
                {
                    ProductIdentifier = siteInfo.ProductIdentifier,
                    Uri = siteInfo.Uri,
                    Title = siteInfo.Title,
                    ImageUrl = siteInfo.ImageUrl
                };

                monitoredProduct.PriceHistory.Add(new PriceChange {Price = siteInfo.Price, ModifiedAt = DateTime.UtcNow});
                monitoredProduct = await this._productRepository.InsertAsync(monitoredProduct);
            }
            else
            {
                var lastEntry = monitoredProduct.PriceHistory.LastOf(x => x.ModifiedAt);
                        
                var newPrice = siteInfo.Price;
                var lastPrice = lastEntry.Price;

                var currentDate = DateTime.UtcNow.Date;
                var lastDate = lastEntry.ModifiedAt.Date;

                if (newPrice != lastPrice || currentDate != lastDate)
                {
                    monitoredProduct.PriceHistory.Add(new PriceChange {Price = siteInfo.Price, ModifiedAt = DateTime.UtcNow});
                    monitoredProduct = await this._productRepository.UpdateAsync(monitoredProduct.Id, monitoredProduct);
                }
            }

            return monitoredProduct;
        }
    }
}