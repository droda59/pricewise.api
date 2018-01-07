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
        private readonly IDocumentLoader _documentLoader;

        public AmazonTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new AmazonSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            var pagesToBrowse = new List<Uri>();
            var desktopRows = document.DocumentNode
                .SelectNodes(".//div[contains(@class, 'desktop-row')]");
            var titleBlocks = desktopRows
                .Select(x => x.SelectSingleNode(".//div[contains(@class, 'as-title-block')]"));

            var hrefs = titleBlocks.Select(titleBLock => titleBLock
                .SelectSingleNode(".//a"))
                .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value));
            pagesToBrowse.AddRange(hrefs);

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);
                if (page.GetElementbyId("zg_centerListWrapper") != null)
                {
                    lock(lockObject)
                    {
                        var listWrapper = page.GetElementbyId("zg_centerListWrapper");
                        var itemWrappers = listWrapper
                            .SelectNodes(".//div[contains(@class, 'zg_itemWrapper')]")
                            .Take(6);
                        var products = itemWrappers
                            .Select(x => x.SelectSingleNode(".//div[contains(@class, 'a-section')]"));
                        var productHrefs = products
                            .Select(x => x.SelectSingleNode(".//a"))
                            .Select(x => new Uri(this.Source.Domain, x.Attributes["href"].Value));

                        productUrls.AddRange(productHrefs);
                    }  
                }
            }));

            return productUrls;
        }
    }
}
