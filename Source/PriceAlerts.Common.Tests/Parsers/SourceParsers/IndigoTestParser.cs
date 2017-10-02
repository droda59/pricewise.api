using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class IndigoTestParser : IndigoParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public IndigoTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new IndigoSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(new Uri(this.Source.Domain, "en-ca/sale/deals-of-the-week"), this.Source.CustomHeaders);
            
            var pagesToBrowse = new List<Uri>();
            pagesToBrowse.AddRange(
                document.DocumentNode
                    .SelectNodes(".//a[@data-type='bannerLink']")
                    .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value)));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);
                if (page.DocumentNode.SelectSingleNode(".//ul[contains(@class, 'product-grid')]") != null)
                {
                    lock(lockObject)
                    {
                        productUrls.AddRange(
                            page.DocumentNode
                                .SelectSingleNode(".//ul[contains(@class, 'product-grid')]")
                                .SelectNodes(".//li[contains(@class, 'product')]//ul[contains(@class, 'product-details')]//li[contains(@class, 'product-title')]//a")
                                .Take(3)
                                .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value)));
                    }  
                }
            }));

            return productUrls;
        }
    }
}
