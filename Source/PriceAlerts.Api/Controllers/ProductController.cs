using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common;
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
        private readonly ISearcher[] _searchers;

        public ProductController(IProductRepository productRepository, IProductFactory productFactory, IHandlerFactory handlerFactory, ISearcher[] searchers)
        {
            this._productRepository = productRepository;
            this._productFactory = productFactory;
            this._handlerFactory = handlerFactory;
            this._searchers = searchers;
        }

        [HttpGet("{productidentifier}")]
        public async Task<IActionResult> FindProductsByIdentifier(string productIdentifier)
        {
            // Do not search for an empty identifier
            if (string.IsNullOrWhiteSpace(productIdentifier))
            {
                return this.NoContent();
            }

            var lockObject = new Object();

            try
            {
                // Get all products with the product identifier from the database
                var knownProducts = await this._productRepository.GetAllByProductIdentifierAsync(productIdentifier);
                var knownProductsSources = knownProducts.Select(x => new Uri(x.Uri).Authority).ToList();

                var newProducts = new List<MonitoredProduct>();

                await Task.WhenAll(this._searchers.Select(async searcher => 
                {
                    // Skip all sources for which we already have the product identifier
                    if (searcher.Source != null && !knownProductsSources.Contains(searcher.Source.Domain.Authority))
                    {
                        var newProductsUrls = await searcher.GetProductsUrls(productIdentifier);
                        await Task.WhenAll(newProductsUrls.Select(async url => 
                        {
                            try
                            {
                                var newProduct = await this._productFactory.CreateProduct(url);
                                lock (lockObject) 
                                {
                                    newProducts.Add(newProduct);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }));
                    }
                }));

                var allProducts = knownProducts.Concat(newProducts).Select(this.CreateProductInfo);

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
    }
}
