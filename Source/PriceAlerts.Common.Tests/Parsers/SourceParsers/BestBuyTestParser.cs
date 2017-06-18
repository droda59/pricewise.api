using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class BestBuyTestParser : BestBuyParser, ITestParser
    {
        public BestBuyTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);
            
            productUrls.AddRange(document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'products-wrapper')]//div[contains(@class, 'layout-item')]//div[contains(@class, 'price-wrapper')]")
                    .SelectMany(x => x.Ancestors("a"))
                    .Select(x => x.Attributes["href"].Value)
                    .Where(x => x.Contains("/product/"))
                    .Select(x => new Uri(this.Domain, x)));

            return productUrls;
        }
    }
}
