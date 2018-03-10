using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class HomeDepotTestParser : HomeDepotParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;
        private readonly HomeDepotSource _source;

        public HomeDepotTestParser(IDocumentLoader documentLoader, HomeDepotSource source)
            : base(documentLoader, source)
        {
            this._documentLoader = documentLoader;
            this._source = source;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();
            var domain = this._source.Domain;

            var document = await this._documentLoader.LoadDocument(domain, this._source.CustomHeaders);
            
            var pagesToBrowse = new List<Uri>();
            pagesToBrowse.AddRange(
                document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'promo') and contains(@class, '-category')]//a")
                    .Select(x => new Uri(domain, x.Attributes["href"].Value)));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this._source.CustomHeaders);
                lock(lockObject)
                {
                    productUrls.AddRange(
                        page.DocumentNode
                            .SelectSingleNode(".//div[contains(@class,'products-grid')]")
                            .SelectNodes(".//div[contains(@class,'item')]//article[contains(@class,'product-card')]")
                            .Take(6)
                            .Select(x => x.SelectSingleNode(".//a"))
                            .Select(x => x.Attributes["href"].Value)
                            .Distinct()
                            .Select(x => new Uri(domain, x)));
                }  
            }));

            return productUrls;
        }
    }
}
