using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class CanadianTireParser : BaseHtmlParser
    {
        private readonly IRequestClient _requestClient;

        public CanadianTireParser(IRequestClient requestClient, IDocumentLoader documentLoader, CanadianTireSource source)
            : base(documentLoader, source)
        {
            this._requestClient = requestClient;
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='pdp-header__main']")
                .SelectSingleNode(".//h1");

            if (titleNode == null)
            {
                titleNode = doc.DocumentNode
                    .SelectSingleNode(".//div[@class='pdp-header__main']")
                    .SelectSingleNode(".//h2");
            }

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                
            var extractedValue = imageNode.Attributes["content"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var price = 0m;

            var productInfoJson = Enumerable.Empty<dynamic>();
            var skuSelector = doc.DocumentNode.SelectSingleNode(".//form[@class='sku-selectors']");
            if (skuSelector != null)
            {
                var indexOfCode = skuSelector.OuterHtml.IndexOf("pCode", StringComparison.InvariantCultureIgnoreCase);
                var productIdentifier = skuSelector.OuterHtml.Substring(indexOfCode + 8, 8).ToUpperInvariant();
                productInfoJson = this.GetProductInfo(productId: productIdentifier);
            }
            
            if (productInfoJson.Any())
            {
                var productEntry = productInfoJson.First();
                price = productEntry.Price;

                if (productEntry.Promo != null) 
                {
                    price = productEntry.Promo.Price;
                }
            }

            return price;
        }

        private IEnumerable<dynamic> GetProductInfo(string sku = null, string productId = null)
        {
            var requestUrl = "/ESB/PriceAvailability?Banner=CTR&Language=E";
            if (!sku.IsNullOrEmpty())
            {
                requestUrl += $"&SKU={sku}";
            }
            else if (!productId.IsNullOrEmpty())
            {
                requestUrl += $"&Product={productId}";
            }
            
            var priceUrl = new Uri(this.Source.Domain, requestUrl);
            var getInfoTask = this._requestClient.ReadHtmlAsync(priceUrl);
            getInfoTask.Wait();

            var infoResult = getInfoTask.Result;

            var infoJson = JsonConvert.DeserializeObject<List<dynamic>>(infoResult);

            return infoJson;
        }
    }
}
