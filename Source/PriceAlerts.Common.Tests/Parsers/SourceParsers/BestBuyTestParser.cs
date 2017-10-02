using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;


namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class BestBuyTestParser : BestBuyParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public BestBuyTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new BestBuySource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            return document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'products-wrapper')]//div[contains(@class, 'layout-item')]//div[contains(@class, 'price-wrapper')]")
                    .SelectMany(x => x.Ancestors("a"))
                    .Select(x => x.Attributes["href"].Value)
                    .Where(x => x.Contains("/product/"))
                    .Select(x => new Uri(this.Source.Domain, x));
        }
    }
}
