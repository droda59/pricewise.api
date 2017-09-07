using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class MonoPriceTestParser : MonoPriceParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public MonoPriceTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new MonoPriceSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(new Uri(this.Source.Domain, "category/cables/hdmi-cables/hdmi-cables"), this.Source.CustomHeaders);

            return document.DocumentNode
                    .SelectNodes("//a[@class='search-result-item-title']")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Select(x => new Uri(x));
        }
    }
}
