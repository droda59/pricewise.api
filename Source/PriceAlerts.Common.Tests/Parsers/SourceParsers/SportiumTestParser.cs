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
            
            var documentNode = document.DocumentNode;
            var banner = documentNode.SelectNodes(".//ul[@class='products-grid with-carousel']");
            var links = banner.SelectMany(x => x.SelectNodes(".//a"));
            var hrefs = links.Select(x => x.Attributes["href"].Value);
            var urls = hrefs.Select(x => new Uri(this.Source.Domain, x));

            // At the moment this product page is not working on Sportium's website
            return urls.Where(
                x => !x.AbsoluteUri
                        .Contains("https://www.sportium.ca/en/ccm-epaulieres-6052-jr-251119-3661200002") && 
                     !x.AbsoluteUri
                        .Contains("https://www.sportium.ca/en/under-armour-chandail-hg-compression-pour-homme-129636"));
        }
    }
}