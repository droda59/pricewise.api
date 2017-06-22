using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class LegoTestParser : LegoParser, ITestParser
    {
        public LegoTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(new Uri(this.Domain, "en-CA/New-Sets"));
            
            productUrls.AddRange(document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'product-leaf')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Select(x => new Uri(this.Domain, x)));

            return productUrls;
        }
    }
}
