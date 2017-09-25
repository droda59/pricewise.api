using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class SearsTestParser : SearsParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public SearsTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new SearsSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            var pagesToBrowse = new List<Uri>();
            pagesToBrowse.AddRange(
                document.DocumentNode
                    .SelectNodes(".//a[contains(@href, 'search?') and (starts-with(@href, 'http'))]")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .OrderBy(elem => Guid.NewGuid())
                    .Take(10)
                    .Select(x => new Uri(x)));

            foreach (var pageUrl in pagesToBrowse)
            {
                Console.WriteLine("loading " + pageUrl.AbsoluteUri);
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);
                if (page.GetElementbyId("search-result-items") != null)
                {
                    var urls = page.GetElementbyId("search-result-items")
                        .SelectNodes(".//li//a[contains(@class, 'product-tile-name-link')]")
                        .Select(x => x.Attributes["href"].Value)
                        .Distinct()
                        .Take(4);

                    foreach (var item in urls)
                    {
                        Uri location;
                        if (Uri.IsWellFormedUriString(item, UriKind.Absolute))
                        {
                            location = new Uri(item);
                        }
                        else
                        {
                            location = new Uri(this.Source.Domain, item);
                        }

                        productUrls.Add(location);
                    }
                }
            }

            return productUrls;
        }
    }
}
