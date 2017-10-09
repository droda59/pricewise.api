using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class BraultMartineauParser : BaseHtmlParser
    {
        public BraultMartineauParser(IDocumentLoader documentLoader, BraultMartineauSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.GetElementbyId("contentWrapper")
                .SelectSingleNode(".//div[contains(@class, 'productMainInfoContainer')]")
                .SelectSingleNode(".//div[contains(@class, 'namePartPriceContainer')]")
                .SelectSingleNode(".//h1");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.GetElementbyId("productMainImage");
                
            var extractedValue = new Uri(this.Source.Domain, imageNode.Attributes["src"].Value);

            return extractedValue.AbsoluteUri;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var wrapper = doc.GetElementbyId("contentWrapper");
            var container = wrapper.SelectSingleNode(".//div[contains(@class, 'productMainInfoContainer')]");
            var partPrice = container.SelectSingleNode(".//div[contains(@class, 'namePartPriceContainer')]");
            var priceNode = partPrice.SelectSingleNode(".//span[contains(@class, 'price')]");

            var dollarsValue = priceNode.SelectSingleNode(".//span[contains(@class, 'price_value')]");
            var centsValue = priceNode.SelectSingleNode(".//sup[contains(@class, 'price_decimal')]");

            var decimalDollarsValue = dollarsValue.InnerText.ExtractDecimal();
            var decimalCentsValue = centsValue.InnerText.ExtractDecimal();

            return decimalDollarsValue + (decimalCentsValue / 100);
        }
    }
}
