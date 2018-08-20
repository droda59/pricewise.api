using System;
using System.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class SailParser : BaseHtmlParser
    {
        public SailParser(IDocumentLoader documentLoader, SailSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override void ParseTitle()
        {
            var title = this.Context.Document.DocumentNode.SelectSingleNode(".//div[@class='product-essential']")
                                .SelectSingleNode(".//div[@class='product-shop']")
                                .SelectSingleNode(".//div[@class='product-name']")
                                .SelectSingleNode(".//span")
                                .InnerText;

            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var nodeValue = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='product-essential']")
                .SelectSingleNode(".//div[@class='product-image-gallery']")
                .SelectSingleNode(".//img[@class='gallery-image visible']");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var documentNode = this.Context.Document.DocumentNode;
            var productEssential = documentNode.SelectSingleNode(".//div[@class='product-essential']");
            var priceBox = productEssential.SelectSingleNode(".//div[@class='price-box']");
            var prices = priceBox.SelectNodes(".//span[@class='price']");
            var priceNode = prices.FirstOrDefault(x => x.Id.Contains("product-price"));
            if (priceNode == null && prices.Count == 1)
            {
                priceNode = prices.First();
            }

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}