using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class CanadianTireTestParser : CanadianTireParser, ITestParser
    {
        private readonly IRequestClient _requestClient;

        public CanadianTireTestParser(IRequestClient requestClient, IDocumentLoader documentLoader)
            : base(requestClient, documentLoader, new CanadianTireSource())
        {
            this._requestClient = requestClient;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var urlsToQuery = new List<Uri>();
            urlsToQuery.Add(new Uri(this.Source.Domain, "/content/canadian-tire/en/homepage-themes/2017/D325C/jcr:content/content-paragraph/recommendations_caro.recommendations.json"));

            await Task.WhenAll(urlsToQuery.Select(async queryUrl => 
            {
                var result = await this._requestClient.ReadHtmlAsync(queryUrl);
                dynamic jsonResult = JsonConvert.DeserializeObject(result);
                foreach (var tile in jsonResult.productTiles)
                {
                    lock(lockObject)
                    {
                        productUrls.Add(new Uri(this.Source.Domain, tile.pdpUrl.ToString()));
                    }  
                }
            }));

            return productUrls;
        }
    }
}
