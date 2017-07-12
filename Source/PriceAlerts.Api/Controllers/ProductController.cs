using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductFactory _productFactory;
        private readonly IParserFactory _parserFactory;
        private readonly ISearcherFactory _searcherFactory;

        public ProductController(IProductRepository productRepository, IProductFactory productFactory, IParserFactory parserFactory, ISearcherFactory searcherFactory)
        {
            this._productRepository = productRepository;
            this._productFactory = productFactory;
            this._parserFactory = parserFactory;
            this._searcherFactory = searcherFactory;
        }

        [HttpGet("{productidentifier}")]
        public async Task<IActionResult> FindProductsByIdentifier(string productIdentifier)
        {
            var lockObject = new Object();

            try
            {
                // Get all products with the product identifier from the database
                var knownProducts = await this._productRepository.GetAllByProductIdentifierAsync(productIdentifier);
                var knownProductsSources = knownProducts.Select(x => new Uri(x.Uri).Authority).ToList();

                var newProducts = new List<MonitoredProduct>();

                await Task.WhenAll(this._searcherFactory.Searchers.Select(async searcher => 
                {
                    // Skip all sources for which we already have the product identifier
                    if (!knownProductsSources.Contains(searcher.Domain.Authority))
                    {
                        var newProductsUrls = await searcher.GetProductsUrls(productIdentifier);
                        await Task.WhenAll(newProductsUrls.Select(async url => 
                        {
                            var newProduct = await this._productFactory.CreateProduct(url);
                            lock (lockObject) 
                            {
                                newProducts.Add(newProduct);
                            }
                        }));
                    }
                }));

                var allProducts = knownProducts.Concat(newProducts);

                return this.Ok(allProducts);
            }
            catch (ParseException e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
