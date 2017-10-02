using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class LeonTestParser : LeonParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public LeonTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new LeonSource())
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
                document.GetElementbyId("home-one")
                    .SelectNodes(".//a[contains(@id, 'link-') and contains(@href, '/search/')]")
                    .Take(6)
                    .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value)));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);
                if (page.GetElementbyId("hawkitemlist") != null)
                {
                    lock(lockObject)
                    {
                        productUrls.AddRange(
                            page.GetElementbyId("hawkitemlist")
                                .SelectNodes(".//div[contains(@class, 'search-product-result')]//div[contains(@class, 'search-pr-text')]//a")
                                .Select(x => x.Attributes["href"].Value)
                                .Distinct()
                                .Take(4)
                                .Select(x => new Uri(this.Source.Domain, x)));
                    }  
                }
            }));

            return productUrls;
        }
    }
}
