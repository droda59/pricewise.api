using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Sources;
using PriceAlerts.Common.Tests.Parsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class SearsTestParser : SearsParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public SearsTestParser(IDocumentLoader documentLoader)
            : base(documentLoader)
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            var pagesToBrowse = new List<Uri>();
            pagesToBrowse.AddRange(
                document.DocumentNode
                    .SelectNodes(".//a[contains(@href, 'search?')]")
                    .Select(x => new Uri(x.Attributes["href"].Value)));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
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

                        lock(lockObject)
                        {
                            productUrls.Add(location);
                        }
                    }
                }
            }));

            return productUrls;
        }
    }
}
