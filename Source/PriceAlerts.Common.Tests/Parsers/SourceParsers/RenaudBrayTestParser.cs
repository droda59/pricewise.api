using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class RenaudBrayTestParser : RenaudBrayParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public RenaudBrayTestParser(IDocumentLoader documentLoader)
            : base(documentLoader)
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            productUrls.AddRange(document.DocumentNode
                    .SelectNodes(".//td[@class='itemTroisProduitsPaginables']//td[contains(@class,'imgCoverContainer')]//a")
                    .Select(x => x.Attributes["href"].Value)
                    .Select(x => new Uri(this.Source.Domain, x)));

            return productUrls;
        }
    }
}
