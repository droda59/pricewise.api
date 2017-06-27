using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class CarcajouTestParser : CarcajouParser, ITestParser
    {
        public CarcajouTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);

            productUrls.AddRange(document.GetElementbyId("nouveautees")
                .SelectNodes(".//div[contains(@class, 'new_prod')]//div[contains(@class, 'new_prod_info')]//a")
                .Select(x => new Uri(this.Domain, x.Attributes["href"].Value)));

            productUrls.AddRange(document.GetElementbyId("meilleurs_vendeurs")
                .SelectNodes(".//div[contains(@class, 'best_seller_prod')]//div[contains(@class, 'best_info')]//a")
                .Select(x => new Uri(this.Domain, x.Attributes["href"].Value)));

            return productUrls;
        }
    }
}
