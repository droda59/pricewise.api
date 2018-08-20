using System;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class MonoPriceParser : BaseHtmlParser
    {
        public MonoPriceParser(IDocumentLoader documentLoader, MonoPriceSource source)
            : base(documentLoader, source)
        {
        }

        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode.SelectSingleNode("//div[@class='product-name']");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var nodeValue = this.Context.Document.GetElementbyId("mono4");
            var extractedValue = nodeValue.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceNode = this.Context.Document.DocumentNode.SelectSingleNode("//span[@class='sale-price']");

            var decimalValue = priceNode.InnerText.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }

        protected override void ParseProductIdentifier()
        {
            var productIdNode = this.Context.Document.DocumentNode.SelectSingleNode("//div[@class='product-code']");
            var text = productIdNode.InnerText;

            this.Context.SitePriceInfo.ProductIdentifier = text.Split('#')[1].Trim();
        }
    }
}
