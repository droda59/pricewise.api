using System;
using System.Collections.Generic;
using System.Linq;

using HtmlAgilityPack;

using Newtonsoft.Json;

namespace PriceAlerts.Common.Parsers
{
    internal class CanadianTireParser : BaseParser, IParser
    {
        private readonly IHtmlLoader _htmlLoader;

        public CanadianTireParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.canadiantire.ca/"))
        {
            this._htmlLoader = htmlLoader;
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='pdp-header__main']")
                .SelectSingleNode(".//h1");

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
            var priceUrl = new Uri($"http://www.canadiantire.ca/ESB/PriceAvailability?SKU={sku}&Banner=CTR&Language=E");
            var getPriceTask = this._htmlLoader.ReadHtmlAsync(priceUrl);
            getPriceTask.Wait();

            var priceResult = getPriceTask.Result;

            var priceJson = JsonConvert.DeserializeObject<List<dynamic>>(priceResult);
            if (priceJson.Any())
            {
                var productEntry = priceJson.First();
                price = productEntry.Price;

                if (productEntry.Promo != null) 
                {
                    price = productEntry.Promo.Price;
                }
            }

            return price;
        }
    }
}
