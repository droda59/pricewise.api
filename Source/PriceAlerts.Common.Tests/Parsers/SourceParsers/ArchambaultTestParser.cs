using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class ArchambaultTestParser : ArchambaultParser, ITestParser
    {
        public ArchambaultTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);
            
            productUrls.AddRange(document.DocumentNode
                    .SelectNodes(".//ul[contains(@class, 'product-list')]//span[contains(@class, 'shadow-behind')]")
                    .Select(x => x.ParentNode.Attributes["href"].Value)
                    .Select(x => new Uri(this.Domain, x)));

            return productUrls;
        }
    }
}
