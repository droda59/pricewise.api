using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class LegoTestParser : LegoParser, ITestParser
    {
        public LegoTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new LegoSource())
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(new Uri(this.Source.Domain, "en-CA/New-Sets"), this.Source.CustomHeaders);

            return document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'product-leaf')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Select(x => new Uri(this.Source.Domain, x));
        }
    }
}
