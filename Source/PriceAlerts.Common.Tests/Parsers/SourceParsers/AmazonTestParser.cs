using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class AmazonTestParser : AmazonHtmlParser, ITestParser
    {
        public AmazonTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new AmazonSource())
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            var pagesToBrowse = new List<Uri>();
            pagesToBrowse.AddRange(
                document.DocumentNode
                    .SelectSingleNode(".//div[contains(@class, 'popular-departments')]")
                    .SelectNodes(".//a")
                    .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value)));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl =>
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);
                if (page.GetElementbyId("mainResults") != null)
                {
                    lock(lockObject)
                    {
                        productUrls.AddRange(
                            page.GetElementbyId("mainResults")
                                .SelectSingleNode(".//ul[contains(@class, 's-result-list')]")
                                .SelectNodes(".//li[contains(@class, 's-result-item')]")
                                .Take(6)
                                .Select(x => x.SelectSingleNode(".//a[contains(@class, 's-access-detail-page')]"))
                                .Select(x => new Uri(x.Attributes["href"].Value)));
                    }
                }
            }));

            return productUrls;
        }
    }
}
