using System;

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

        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.GetElementbyId("contentWrapper")
                .SelectSingleNode(".//div[contains(@class, 'productMainInfoContainer')]")
                .SelectSingleNode(".//div[contains(@class, 'namePartPriceContainer')]")
                .SelectSingleNode(".//h1");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var imageNode = this.Context.Document.GetElementbyId("productMainImage");
                
            var extractedValue = new Uri(this.Source.Domain, imageNode.Attributes["src"].Value);

            this.Context.SitePriceInfo.ImageUrl = extractedValue.AbsoluteUri;
        }

        protected override void ParsePrice()
        {
            var wrapper = this.Context.Document.GetElementbyId("contentWrapper");
            var container = wrapper.SelectSingleNode(".//div[contains(@class, 'productMainInfoContainer')]");
            var partPrice = container.SelectSingleNode(".//div[contains(@class, 'namePartPriceContainer')]");
            var priceNode = partPrice.SelectSingleNode(".//span[contains(@class, 'price')]");

            var dollarsValue = priceNode.SelectSingleNode(".//span[contains(@class, 'price_value')]");
            var centsValue = priceNode.SelectSingleNode(".//sup[contains(@class, 'price_decimal')]");

            var decimalDollarsValue = dollarsValue.InnerText.ExtractDecimal();
            var decimalCentsValue = centsValue.InnerText.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalDollarsValue + (decimalCentsValue / 100);
        }
    }
}
