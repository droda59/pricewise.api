using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class TigerDirectTestParser : TigerDirectParser, ITestParser
    {
        public TigerDirectTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);
            
            productUrls.AddRange(document.GetElementbyId("homeFeatured")
                    .SelectNodes(".//div[contains(@class, 'product')]//div[contains(@class, 'productInfo')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Select(x => new Uri(this.Domain, x)));

            return productUrls;
        }
    }
}
