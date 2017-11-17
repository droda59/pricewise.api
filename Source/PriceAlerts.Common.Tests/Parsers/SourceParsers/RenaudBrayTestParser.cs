using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class RenaudBrayTestParser : RenaudBrayParser, ITestParser
    {
        public RenaudBrayTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new RenaudBraySource())
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            return document.DocumentNode
                    .SelectNodes(".//td[@class='itemTroisProduitsPaginables']//td[contains(@class,'imgCoverContainer')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Select(x => new Uri(this.Source.Domain, x));
        }
    }
}
