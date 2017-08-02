using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class LegoTestParser : LegoParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public LegoTestParser(IDocumentLoader documentLoader)
            : base(documentLoader)
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(new Uri(this.Source.Domain, "en-CA/New-Sets"), this.Source.CustomHeaders);
            
            productUrls.AddRange(document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'product-leaf')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Select(x => new Uri(this.Source.Domain, x)));

            return productUrls;
        }
    }
}
