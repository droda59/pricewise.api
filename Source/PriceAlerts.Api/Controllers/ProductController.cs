using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.CommandHandlers;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductFactory _productFactory;
        private readonly IHandlerFactory _handlerFactory;
        private readonly IEnumerable<ICommandHandler> _handlers;

        public ProductController(IProductRepository productRepository, IProductFactory productFactory, IHandlerFactory handlerFactory, IEnumerable<ICommandHandler> handlers)
        {
            this._productRepository = productRepository;
            this._productFactory = productFactory;
            this._handlerFactory = handlerFactory;
            this._handlers = handlers;
        }

        [HttpPost]
        [LoggingDescription("Request to find products")]
        public virtual async Task<IActionResult> FindProductsByIdentifier([FromBody] string[] productIdentifiers)
        {
            // Do not search for an empty identifier
            if (!productIdentifiers.Any())
            {
                return this.NoContent();
            }
            
            var lockObject = new object();

            try
            {
                var knownProducts = new List<MonitoredProduct>();
                var newProducts = new List<MonitoredProduct>();
                foreach (var productIdentifier in productIdentifiers)
                {
                    // Get all products with the product identifier from the database
                    var productIdentifierProducts = (await this._productRepository.GetAllByProductIdentifierAsync(productIdentifier)).ToList();
                    var knownProductsSources = productIdentifierProducts.Select(x => new Uri(x.Uri).Authority).ToList();
                    
                    knownProducts.AddRange(productIdentifierProducts);

                    await Task.WhenAll(this._handlers.Select(async handler =>
                    {
                        if (!knownProductsSources.Contains(handler.Domain.Authority))
                        {
                            var newProductsUrls = await handler.HandleSearch(productIdentifier);
                            foreach (var url in newProductsUrls)
                            {
                                try
                                {
                                    var newProduct = await this.ForceGetProduct(handler, url);
                                    lock (lockObject)
                                    {
                                        newProducts.Add(newProduct);
                                    }
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                        }
                    }));
                }

                var allProducts = knownProducts.Concat(newProducts).DistinctBy(x => x.Uri).Select(this.CreateProductInfo);

                return this.Ok(allProducts);
            }
            catch (ParseException e)
            {
                return this.BadRequest(e.Message);
            }
        }

        private ProductInfo CreateProductInfo(MonitoredProduct product)
        {
            var originalUrl = new Uri(product.Uri);
            var handler = this._handlerFactory.CreateHandler(originalUrl);
            
            return new ProductInfo
            {
                OriginalUrl = originalUrl.AbsoluteUri,
                ProductUrl = handler.HandleManipulateUrl(originalUrl).AbsoluteUri,
                Title = product.Title,
                Price = product.PriceHistory.LastOf(y => y.ModifiedAt).Price,
                LastUpdate = product.PriceHistory.LastOf(y => y.ModifiedAt).ModifiedAt,
                ImageUrl = product.ImageUrl,
                ProductIdentifier = product.ProductIdentifier
            };
        }

        private async Task<MonitoredProduct> ForceGetProduct(ICommandHandler handler, Uri url)
        {
            var cleanUrl = handler.HandleCleanUrl(url);
            var existingProduct = await this._productRepository.GetByUrlAsync(cleanUrl.AbsoluteUri);
            if (existingProduct == null)
            {
                existingProduct = await this._productFactory.CreateProduct(url);
            }

            return existingProduct;
        }
    }
}
