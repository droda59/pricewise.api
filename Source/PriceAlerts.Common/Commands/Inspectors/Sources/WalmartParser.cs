using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class WalmartParser : BaseHtmlParser
    {
        public WalmartParser(IDocumentLoader documentLoader, WalmartSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@id='product-desc']//h1");
            var extractedValue = titleNode.FirstChild.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var image = doc.DocumentNode.SelectSingleNode("//div[@id='product-images']//div[@class='centered-img-wrap']//img");
            var extractedValue = new Uri(this.Source.Domain, image.Attributes["src"].Value);

            return extractedValue.AbsoluteUri;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            // We need to use the async method
            return 0;
        }

        protected override async Task<decimal> GetPriceAsync(HtmlDocument doc)
        {
            var customHeaders = new List<KeyValuePair<string,string>>();
            customHeaders.Add(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"));

            var skuPosition = doc.DocumentNode.InnerHtml.IndexOf("\"sku\"") + 7;
            var skuString = doc.DocumentNode.InnerHtml.Substring(skuPosition, 13);

            var upcPosition = doc.DocumentNode.InnerHtml.IndexOf("\"upc\"") +8;
            var upcString = doc.DocumentNode.InnerHtml.Substring(upcPosition, 15);

            var priceUri = new Uri("https://www.walmart.ca/ws/fr/products/availability-pip");
            var requestBody = "{\"stores\":[\"1170\"], \"products\":{\""+skuString + "\":[{\"sku\":\""+skuString+"\",\"upc\":[\""+upcString+"\"]}]},\"origin\":\"pip\"}";

            var priceRequest = await this._documentLoader.LoadDocumentAsString(priceUri, HttpMethod.Post, requestBody, "application/json", customHeaders);
            var priceObject = JObject.Parse(priceRequest);

            var price = (decimal)priceObject.SelectToken(skuString+".onlineSummary.minCurrentPrice");
            return price;
        }
    }
}
