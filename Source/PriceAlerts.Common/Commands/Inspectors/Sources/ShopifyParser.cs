using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nager.AmazonProductAdvertising.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class ShopifyParser : IInspector
    {
        private readonly IRequestClient _requestClient;
        private readonly KeyValuePair<string, string>[] _customHeaders;

        public ShopifyParser(IRequestClient requestClient)
        {
            this._requestClient = requestClient;
            this._customHeaders = new[] 
            {
                new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36")
            };
        }

        [LoggingDescription("Parsing product page")]
        public async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            var result = await this._requestClient.ReadHtmlAsync(new Uri(url, "products.json"), this._customHeaders);

            var resultToken = JToken.Parse(result);
            
            if (resultToken == null)
            {
                throw new ParseException("Error parsing the document", url);
            }
            
            var sitePriceInfo =  new SitePriceInfo()
            {
                ProductIdentifier = resultToken.SelectToken("product.id").Value<string>(),
                Uri = url.AbsoluteUri,
                ImageUrl = resultToken.SelectToken("product.image.src").Value<string>(),
                Price = resultToken.SelectToken("product.variants[0].price").Value<string>()?.ExtractDecimal() ?? 0,
                Title = resultToken.SelectToken("product.title").Value<string>()
            };

            if (sitePriceInfo.Price == 0)
            {
                throw new ParseException("Error parsing the price", new Exception("Price not found"), url);
            }

            return sitePriceInfo;
        }
    }
}