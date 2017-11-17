using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class TigerDirectTestParser : TigerDirectParser, ITestParser
    {
        public TigerDirectTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new TigerDirectSource())
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            return document.GetElementbyId("homeFeatured")
                    .SelectNodes(".//div[contains(@class, 'product')]//div[contains(@class, 'productInfo')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Select(x => new Uri(this.Source.Domain, x));
        }
    }
}
