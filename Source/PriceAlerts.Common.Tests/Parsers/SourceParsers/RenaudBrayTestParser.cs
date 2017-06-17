using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class RenaudBrayTestParser : RenaudBrayParser, ITestParser
    {
        public RenaudBrayTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);
            
            productUrls.AddRange(document.DocumentNode
                    .SelectNodes(".//td[@class='itemTroisProduitsPaginables']//td[contains(@class,'imgCoverContainer')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Select(x => new Uri(this.Domain, x)));

            return productUrls;
        }
    }
}
