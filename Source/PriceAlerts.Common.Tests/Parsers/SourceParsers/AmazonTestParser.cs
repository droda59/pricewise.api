using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Tests.Parsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class AmazonTestParser : AmazonParser, ITestParser
    {
        public AmazonTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);
            
            var pagesToBrowse = new List<Uri>();
            pagesToBrowse.AddRange(
                document.DocumentNode
                    .SelectSingleNode(".//div[contains(@class, 'popular-departments')]")
                    .SelectNodes(".//a")
                    .Select(x => new Uri(this.Domain, x.Attributes["href"].Value)));

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this.LoadDocument(pageUrl);
                if (page.GetElementbyId("mainResults") != null)
                {
                    lock(lockObject)
                    {
                        productUrls.AddRange(
                            page.GetElementbyId("mainResults")
                                .SelectSingleNode(".//ul[contains(@class, 's-result-list')]")
                                .SelectNodes(".//li[contains(@class, 's-result-item')]")
                                .Take(10)
                                .Select(x => x.SelectSingleNode(".//a[contains(@class, 's-access-detail-page')]"))
                                .Select(x => new Uri(x.Attributes["href"].Value)));
                    }  
                }
            }));

            return productUrls;
        }
    }
}
