using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class NcixTestParser : NcixParser, ITestParser
    {
        public NcixTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new NcixSource())
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(new Uri(this.Source.Domain, "https://www.ncix.com/category/computer-cases-53-104.htm"), this.Source.CustomHeaders);

            return document.DocumentNode
                .SelectNodes("//span[@class='listing'] //a")
                .Select(x => x.Attributes["href"].Value)
                .Distinct()
                .Select(x => new Uri(x))
                .Take(10);
        }
    }
}
