using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Newtonsoft.Json;

using PriceAlerts.Common.Parsers.SourceParsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class CanadianTireTestParser : CanadianTireParser, ITestParser
    {
        private readonly IHtmlLoader _htmlLoader;
        
        public CanadianTireTestParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
            this._htmlLoader = htmlLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var document = await this.LoadDocument(this.Domain);

            var urlsToQuery = new List<Uri>();
            urlsToQuery.Add(new Uri(this.Domain, "/content/canadian-tire/en/homepage-themes/2017/D325C/jcr:content/content-paragraph/recommendations_caro.recommendations.json"));

            // var lazyLoadedNodes = document.DocumentNode.SelectNodes(".//div[contains(@class, 'recently-viewed-product-tiles') and contains(@class, 'lazyload')]");
            // foreach (var node in lazyLoadedNodes.Take(6))
            // {
            //     if (node.Attributes["data-config"] != null)
            //     {
            //         dynamic configValue = JsonConvert.DeserializeObject(node.Attributes["data-config"].Value);
            //         Console.WriteLine(configValue);
            //         var urlToQuery = new Uri(this.Domain, configValue.currentPath + ".recommendations.json");
            //         urlsToQuery.Add(urlToQuery);
            //     }
            // }

            await Task.WhenAll(urlsToQuery.Select(async queryUrl => 
            {
                var result = await this._htmlLoader.ReadHtmlAsync(queryUrl);
                dynamic jsonResult = JsonConvert.DeserializeObject(result);
                foreach (var tile in jsonResult.productTiles)
                {
                    lock(lockObject)
                    {
                        productUrls.Add(new Uri(this.Domain, tile.pdpUrl.ToString()));
                    }  
                }
            }));

            return productUrls;
        }
    }
}
