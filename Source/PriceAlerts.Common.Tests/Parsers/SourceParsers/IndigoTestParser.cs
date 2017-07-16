using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class IndigoTestParser : IndigoParser, ITestParser
    {
        public IndigoTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(new Uri(this.Domain, "en-ca/sale/deals-of-the-week"));
            
            var pagesToBrowse = new List<Uri>();
            pagesToBrowse.AddRange(
                document.DocumentNode
                    .SelectNodes(".//a[@data-type='bannerLink']")
                    .Select(x => new Uri(this.Domain, x.Attributes["href"].Value)));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this.LoadDocument(pageUrl);
                if (page.DocumentNode.SelectSingleNode(".//ul[contains(@class, 'product-grid')]") != null)
                {
                    lock(lockObject)
                    {
                        productUrls.AddRange(
                            page.DocumentNode
                                .SelectSingleNode(".//ul[contains(@class, 'product-grid')]")
                                .SelectNodes(".//li[contains(@class, 'product')]//ul[contains(@class, 'product-details')]//li[contains(@class, 'product-title')]//a")
                                .Take(3)
                                .Select(x => new Uri(this.Domain, x.Attributes["href"].Value)));
                    }  
                }
            }));

            return productUrls;
        }
    }
}
