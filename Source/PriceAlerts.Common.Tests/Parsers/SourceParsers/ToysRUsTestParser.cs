using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class ToysRUsTestParser : ToysRUsParser, ITestParser
    {
        public ToysRUsTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);
            
            var pagesToBrowse = document.DocumentNode
                    .SelectNodes(".//div[contains(@class, 'product-carousel')]//div[contains(@class, 'slide')]/a")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Select(x => new Uri(x));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this.LoadDocument(pageUrl);

                lock(lockObject)
                {
                    productUrls.AddRange(
                        page.DocumentNode
                            .SelectSingleNode(".//ul[contains(@class, 'product-list')]")
                            .SelectNodes(".//div[contains(@class, 'productDesc')]/a")
                            .Take(6)
                            .Select(x => x.Attributes["href"].Value)
                            .Select(x => new Uri(this.Domain, x)));
                }  
            }));

            return productUrls;
        }
    }
}
