using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class HomeDepotParser : IInspector
    {
        private readonly IRequestClient _requestClient;
        private readonly IDocumentLoader _documentLoader;
        private readonly HomeDepotSource _source;

        protected HomeDepotParser(IRequestClient requestClient, IDocumentLoader documentLoader, HomeDepotSource source)
        {
            this._requestClient = requestClient;
            this._documentLoader = documentLoader;

            this._source = source;
        }

        [LoggingDescription("Parsing HTML")]
        public async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            var document = await this._documentLoader.LoadDocument(url, this._source.CustomHeaders);

            if (document == null)
            {
                throw new ParseException("Error parsing the document", url);                
            }
            
            var sku = this.GetProductSku(document);
            var productInfo = this.GetProductInfo(sku);
            var priceInfo = this.GetProductPrice(sku);
            
            var title = productInfo.Name;
            var imageUrl = productInfo.Images.First().Url;

            var price = 0m;
            if (priceInfo.OptimizedPrice.DisplayPrice != null)
            {
                price = priceInfo.OptimizedPrice.DisplayPrice.Value;
            }
            else
            {
                price = productInfo.Price.Value;
            }

            return new SitePriceInfo
            {
                ProductIdentifier = string.Empty, 
                Uri = url.AbsoluteUri,
                Title = title,
                ImageUrl = imageUrl,
                Price = price
            };
        }

        private HomeDepotProductInfo GetProductInfo(string sku)
        {
            var priceUrl = new Uri(this._source.Domain, $"/homedepotcacommercewebservices/v2/homedepotca/products/{sku}/");
            var getInfoTask = this._requestClient.ReadHtmlAsync(priceUrl);
            getInfoTask.Wait();

            var infoResult = getInfoTask.Result;

            return JsonConvert.DeserializeObject<HomeDepotProductInfo>(infoResult);
        }

        private HomeDepotProductPriceInfo GetProductPrice(string sku)
        {
            var priceUrl = new Uri(this._source.Domain, $"/homedepotcacommercewebservices/v2/homedepotca/products/{sku}/localized/7159/");
            var getInfoTask = this._requestClient.ReadHtmlAsync(priceUrl);
            getInfoTask.Wait();

            var infoResult = getInfoTask.Result;

            return JsonConvert.DeserializeObject<HomeDepotProductPriceInfo>(infoResult);
        }

        private string GetProductSku(HtmlDocument doc)
        {
            var productData = doc.GetElementbyId("jsProductData");
            var dataCode = productData.Attributes["data-code"].Value;
            if (this._source.SkuExpression.IsMatch(dataCode))
            {
                return dataCode;
            }

            return string.Empty;
        }

        private class HomeDepotProductInfo
        {
            public HomeDepotProductInfo()
            {
                this.Images = new List<HomeDepotProductImageInfo>();
            }
            
            public string Name { get; set; }
            
            public List<HomeDepotProductImageInfo> Images { get; set; }
            
            public HomeDepotPriceInfo Price { get; set; }
        }

        private class HomeDepotProductImageInfo
        {
            public string Url { get; set; }
        }

        private class HomeDepotProductPriceInfo
        {
            public HomeDepotOptimizedPriceInfo OptimizedPrice { get; set; }
        }

        private class HomeDepotOptimizedPriceInfo
        {
            public HomeDepotPriceInfo DisplayPrice { get; set; }
        }

        private class HomeDepotPriceInfo
        {
            public decimal Value { get; set; }
        }
    }
}