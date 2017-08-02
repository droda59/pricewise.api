using System;
using System.Collections.Generic;
using System.Linq;

using HtmlAgilityPack;

using Newtonsoft.Json;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    public class CanadianTireParser : BaseParser, IParser
    {
        private readonly IRequestClient _requestClient;

        public CanadianTireParser(IRequestClient requestClient, IDocumentLoader documentLoader)
            : base(documentLoader, new CanadianTireSource())
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
            decimal price = 0m;

            var sku = doc.DocumentNode.SelectSingleNode(".//div[@class='sku-selectors__fieldset-wrapper']").Attributes["data-sku"].Value.Substring(0, 7);
            var infoJson = this.GetProductInfo(sku);
            if (infoJson.Any())
            {
                var productEntry = infoJson.First();
                price = productEntry.Price;

                if (productEntry.Promo != null) 
                {
                    price = productEntry.Promo.Price;
                }
            }

            return price;
        }

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var productIdentifier = string.Empty;

            // var sku = doc.DocumentNode.SelectSingleNode(".//div[@class='sku-selectors__fieldset-wrapper']").Attributes["data-sku"].Value.Substring(0, 7);
            // var infoJson = this.GetProductInfo(sku);
            // if (infoJson.Any())
            // {
            //     var productEntry = infoJson.First();
            //     productIdentifier = productEntry.PartNumber;
            // }

            return productIdentifier;
        }

        private IEnumerable<dynamic> GetProductInfo(string sku)
        {
            var priceUrl = new Uri(this.Source.Domain, $"/ESB/PriceAvailability?SKU={sku}&Banner=CTR&Language=E");
            var getInfoTask = this._requestClient.ReadHtmlAsync(priceUrl);
            getInfoTask.Wait();

            var infoResult = getInfoTask.Result;

            var infoJson = JsonConvert.DeserializeObject<List<dynamic>>(infoResult);

            return infoJson;
        }
    }
}
