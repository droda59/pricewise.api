using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class CarcajouTestParser : CarcajouParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public CarcajouTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new CarcajouSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            productUrls.AddRange(document.GetElementbyId("nouveautees")
                .SelectNodes(".//div[contains(@class, 'new_prod')]//div[contains(@class, 'new_prod_info')]//a")
                .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value)));

            productUrls.AddRange(document.GetElementbyId("meilleurs_vendeurs")
                .SelectNodes(".//div[contains(@class, 'best_seller_prod')]//div[contains(@class, 'best_info')]//a")
                .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value)));

            return productUrls;
        }
    }
}
