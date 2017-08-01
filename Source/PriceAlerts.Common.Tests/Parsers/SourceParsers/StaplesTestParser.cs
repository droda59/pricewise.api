using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Sources;
using PriceAlerts.Common.Tests.Parsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class StaplesTestParser : StaplesParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public StaplesTestParser(IDocumentLoader documentLoader)
            : base(documentLoader)
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            // productUrls.AddRange(document.GetElementbyId("trendingProductsSection")
            //     .SelectNodes(".//div[contains(@class,'stp--new-product-tile-container')]//div[contains(@class, 'product-details')]//a")
            //     .Take(6)
            //     .Select(x => x.Attributes["href"].Value)
            //     .Select(x => new Uri(this.Source.Domain, x)));

            productUrls.AddRange(document.GetElementbyId("newProductsSection")
                .SelectNodes(".//div[contains(@class,'stp--new-product-tile-container')]//div[contains(@class, 'product-details')]/a")
                .Select(x => x.Attributes["href"].Value)
                .Select(x => new Uri(this.Source.Domain, x)));
            
            // productUrls.AddRange(document.GetElementbyId("recommendedProductsSection")
            //     .SelectNodes(".//div[contains(@class,'stp--new-product-tile-container')]//div[contains(@class, 'product-details')]/a")
            //     .Select(x => x.Attributes["href"].Value)
            //     .Select(x => new Uri(this.Source.Domain, x)));

            return productUrls;
        }
    }
}
