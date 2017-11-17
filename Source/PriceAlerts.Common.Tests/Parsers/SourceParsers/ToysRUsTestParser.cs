using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class ToysRUsTestParser : ToysRUsParser, ITestParser
    {
        public ToysRUsTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new ToysRUsSource())
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            var pagesToBrowse = document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'product-carousel')]//div[contains(@class, 'slide')]/a")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Select(x => new Uri(x));

            await Task.WhenAll(pagesToBrowse.Where(x => x.AbsoluteUri.Contains("categoryId")).Select(async pageUrl =>
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);

                lock(lockObject)
                {
                    productUrls.AddRange(
                        page.DocumentNode
                            .SelectNodes(".//ul[contains(@class, 'product-list')]//div[contains(@class, 'productDesc')]/a")
                            .Take(6)
                            .Select(x => x.Attributes["href"].Value)
                            .Select(x => new Uri(this.Source.Domain, x)));
                }
            }));

            return productUrls;
        }
    }
}
