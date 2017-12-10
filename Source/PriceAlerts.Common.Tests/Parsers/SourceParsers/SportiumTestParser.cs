using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class SportiumTestParser : SportiumParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public SportiumTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new SportiumSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            var singleNode = document.DocumentNode.SelectSingleNode(".//div[@class='after-content-container']");
            var nodes = singleNode.SelectNodes(".//div[@class='productbox-banner']");
            var urls = nodes.SelectMany(x => x.SelectNodes("a"))
                .Select(x => x.Attributes["href"].Value)
                .Select(x => new Uri(this.Source.Domain, x));
            return urls;
        }
    }
}