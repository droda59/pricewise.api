using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class StaplesTestParser : StaplesParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public StaplesTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new StaplesSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            return document.GetElementbyId("newProductsSection")
                .SelectNodes(".//div[contains(@class,'stp--new-product-tile-container')]//div[contains(@class, 'product-details')]/a")
                .Select(x => x.Attributes["href"].Value)
                .Select(x => new Uri(this.Source.Domain, x));
        }
    }
}
