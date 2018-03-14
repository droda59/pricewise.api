using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class SailTestParser : SailParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public SailTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new SailSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            var urls = document.DocumentNode
                .SelectNodes(".//div[@class='widget-multi-products']")
                .First()
                .SelectSingleNode(".//div[@class='products-carousel products-carousel-slick']")
                .SelectNodes(".//h3[@class='product-name']")
                .SelectMany(x => x.SelectNodes("a"))
                .Select(x => x.Attributes["href"].Value)
                .Select(x => new Uri(this.Source.Domain, x));
            return urls;
        }
    }
}