using System;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class LaCordeeParser : BaseHtmlParser
    {
        public LaCordeeParser(IDocumentLoader documentLoader, LaCordeeSource source)
            : base(documentLoader, source)
        {
        }

        protected override void ParseTitle()
        {
            var documentNode = this.Context.Document.DocumentNode;
            var itemDetails = documentNode.SelectSingleNode(".//div[@class='item-details block']");
            var textNode = itemDetails.SelectSingleNode(".//h1");
            var title = textNode.InnerText;

            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title =  extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var documentNode = this.Context.Document.DocumentNode;
            var productImage = documentNode.SelectSingleNode(".//p[@class='product-image']");
            var imageNode = productImage.SelectSingleNode(".//img");

            var extractedValue = imageNode.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceBox = this.Context.Document.DocumentNode.SelectSingleNode(".//div[@class='price-box']");
            var specialPrice = priceBox.SelectSingleNode(".//p[@class='special-price']");

            var priceNode = specialPrice == null ?
                priceBox.SelectSingleNode(".//span[@class='price']") :
                specialPrice.SelectSingleNode(".//span[@class='price']");

            // LaCordee sometimes displays a range of prices for a single product. We sadly have to way of determining the correct price for the product.
            if (priceNode.InnerText != null && (priceNode.InnerText.Contains("à") || priceNode.InnerText.Contains("to")))
            {
                throw new NotSupportedException("PriceWise was unable to determine the correct price for the selected product.");
            }

            this.Context.SitePriceInfo.Price = priceNode.InnerText.ExtractDecimal();
        }
    }
}