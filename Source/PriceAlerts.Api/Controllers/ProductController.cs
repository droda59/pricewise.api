using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Commands.Searchers;
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
        private readonly IEnumerable<ISearcher> _searchers;

        public ProductController(IProductRepository productRepository, IProductFactory productFactory, IHandlerFactory handlerFactory, IEnumerable<ISearcher> searchers)
        {
            this._productRepository = productRepository;
            this._productFactory = productFactory;
            this._handlerFactory = handlerFactory;
            this._searchers = searchers;
        }

        [HttpPost]
        public async Task<IActionResult> FindProductsByIdentifier([FromBody] string[] productIdentifiers)
        {
            // Do not search for an empty identifier
            if (!productIdentifiers.Any())
            {
                return this.NoContent();
            }

            try
            {
                var knownProducts = new List<MonitoredProduct>();
                var newProducts = new List<MonitoredProduct>();
                foreach (var productIdentifier in productIdentifiers)
                {
                    // Get all products with the product identifier from the database
                    var productIdentifierProducts = (await this._productRepository.GetAllByProductIdentifierAsync(productIdentifier)).ToList();
                    knownProducts.AddRange(productIdentifierProducts);

                    foreach (var searcher in this._searchers)
                    {
                        var newProductsUrls = await searcher.GetProductsUrls(productIdentifier);
                        foreach (var url in newProductsUrls.ToList())
                        {
                            try
                            {
                                var newProduct = await this.ForceGetProduct(url);
                                newProducts.Add(newProduct);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    }
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
        
        private async Task<MonitoredProduct> ForceGetProduct(Uri url)
        {
            var cleanUrl = this._handlerFactory.CreateHandler(url).HandleCleanUrl(url);
            var existingProduct = await this._productRepository.GetByUrlAsync(cleanUrl.AbsoluteUri);
            if (existingProduct == null)
            {
                existingProduct = await this._productFactory.CreateProduct(url);
            }

            return existingProduct;
        }
    }
}
