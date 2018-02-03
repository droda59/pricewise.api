using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nager.AmazonProductAdvertising.Model;

using Newtonsoft.Json;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class ShopifyParser : IInspector
    {
        private readonly IRequestClient _requestClient;
        private readonly ShopifySource _shopifySource;

        public ShopifyParser(IRequestClient requestClient, ShopifySource shopifySource)
        {
            this._requestClient = requestClient;
            this._shopifySource = shopifySource;
        }

        [LoggingDescription("Parsing product page")]
        public async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            var result = await this._requestClient.ReadHtmlAsync(new Uri(url, "products.json"), this._shopifySource.CustomHeaders.ToArray());
            
            var product = JsonConvert.DeserializeObject<Product>(result);

            if (product == null)
            {
                throw new ParseException("Error parsing the document", url);
            }
            
            var sitePriceInfo =  new SitePriceInfo()
            {
                ProductIdentifier = product?.Id,
                Uri = url.AbsoluteUri,
                ImageUrl = product?.Image?.Src,
                Price = product?.Variants?.FirstOrDefault()?.Price?.ExtractDecimal() ?? 0,
                Title = product?.Title
            };

            if (sitePriceInfo.Price == 0)
            {
                throw new ParseException("Error parsing the price", new Exception("Price not found"), url);
            }

            return sitePriceInfo;
        }

        #region Deserialization

        private class Product
        {
            public string Id { get; set; }

            public string Title { get; set; }

            public IEnumerable<Variant> Variants { get; set; }

            public Image Image { get; set; }
        }
        
        private class Variant
        {
            public string Price { get; set; }
        }
        
        private class Image
        {
            public string Src { get; set; }
        }

        #endregion
    }
}