using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;
using HttpClient = System.Net.Http.HttpClient;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class HomeDepotParser : IParser
    {
        private readonly IDocumentLoader _documentLoader;
        private readonly HomeDepotSource _source;

        protected HomeDepotParser(IDocumentLoader documentLoader, HomeDepotSource source)
        {
            this._documentLoader = documentLoader;

            this._source = source;
        }

        [LoggingDescription("Parsing HTML")]
        public async Task<SitePriceInfo> Parse(Uri url)
        {
            var document = await this._documentLoader.LoadDocument(url, this._source.CustomHeaders);
            if (document == null)
            {
                throw new ParseException("Error parsing the document", url);                
            }

            var sku = this.GetProductSku(document);
            using (var httpClient = new HttpClient { BaseAddress = this._source.Domain })
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var productInfo = await GetProductInfo(httpClient, sku);
                var priceInfo = await GetProductPrice(httpClient, sku);
                
                var title = productInfo.Name;
                var imageUrl = $"https://s7d2.scene7.com/is/image/homedepotcanada/p_{sku}.jpg";
                var price = priceInfo.OptimizedPrice.DisplayPrice?.Value ?? productInfo.Price.Value;

                ServicePointManager.ServerCertificateValidationCallback = null;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
                
                var sitePriceInfo = new SitePriceInfo
                {
                    ProductIdentifier = string.Empty, 
                    Uri = url.AbsoluteUri,
                    Title = title,
                    ImageUrl = imageUrl,
                    Price = price
                };
                
                return sitePriceInfo;
            }
        }

        private static async Task<HomeDepotProductInfo> GetProductInfo(HttpClient httpClient, string sku)
        {
            var productInfoUrl = $"/homedepotcacommercewebservices/v2/homedepotca/products/{sku}/";

            var response = await httpClient.GetAsync(productInfoUrl);
            var infoResult = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<HomeDepotProductInfo>(infoResult);
        }

        private static async Task<HomeDepotProductPriceInfo> GetProductPrice(HttpClient httpClient, string sku)
        {
            var priceUrl = $"/homedepotcacommercewebservices/v2/homedepotca/products/{sku}/localized/7159/";
            
            var response2 = await httpClient.GetAsync(priceUrl);
            var priceResult = await response2.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<HomeDepotProductPriceInfo>(priceResult);
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