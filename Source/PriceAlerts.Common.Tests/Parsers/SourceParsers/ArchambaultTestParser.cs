using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class ArchambaultTestParser : ArchambaultParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public ArchambaultTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new ArchambaultSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            return document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'products-carousel')]//div[contains(@class, 'carousel__item-description')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Take(24)
                    .Select(x => new Uri(this.Source.Domain, x));
        }
    }
}
