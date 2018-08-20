using System;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class RonaParser : BaseHtmlParser
    {
        public RonaParser(IDocumentLoader documentLoader, RonaSource source)
            : base(documentLoader, source)
        {
        }

        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode.SelectSingleNode("//meta[@property='og:title']");

            var extractedValue = titleNode.Attributes["content"].Value.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var imageNode = this.Context.Document.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                
            var extractedValue = imageNode.Attributes["content"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceContentNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'productDetails')]")
                .SelectSingleNode(".//span[contains(@class, 'price-box__price')]")
                .SelectSingleNode(".//span[contains(@class, 'price-box__price__amount')]");

            var dollarsValue = priceContentNode.SelectSingleNode(".//span[contains(@class, 'price-box__price__amount__integer')]");
            var centsValue = priceContentNode.SelectSingleNode(".//sup[contains(@class, 'price-box__price__amount__decimal')]");

            var decimalDollarsValue = dollarsValue?.InnerText.Replace(",", string.Empty).ExtractDecimal() ?? 0m;
            var decimalCentsValue = centsValue?.InnerText.ExtractDecimal() ?? 0m;

            this.Context.SitePriceInfo.Price = decimalDollarsValue + (decimalCentsValue / 100);
        }
    }
}