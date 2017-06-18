using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Tests.Parsers;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    internal class StaplesTestParser : StaplesParser, ITestParser
    {
        public StaplesTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);
            
            // productUrls.AddRange(document.GetElementbyId("trendingProductsSection")
            //     .SelectNodes(".//div[contains(@class,'stp--new-product-tile-container')]//div[contains(@class, 'product-details')]//a")
            //     .Take(6)
            //     .Select(x => x.Attributes["href"].Value)
            //     .Select(x => new Uri(this.Domain, x)));

            productUrls.AddRange(document.GetElementbyId("newProductsSection")
                .SelectNodes(".//div[contains(@class,'stp--new-product-tile-container')]//div[contains(@class, 'product-details')]/a")
                .Select(x => x.Attributes["href"].Value)
                .Select(x => new Uri(this.Domain, x)));
            
            // productUrls.AddRange(document.GetElementbyId("recommendedProductsSection")
            //     .SelectNodes(".//div[contains(@class,'stp--new-product-tile-container')]//div[contains(@class, 'product-details')]/a")
            //     .Select(x => x.Attributes["href"].Value)
            //     .Select(x => new Uri(this.Domain, x)));

            return productUrls;
        }
    }
}
