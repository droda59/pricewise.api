using System;
using System.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class SportiumParser : BaseHtmlParser
    {
        public SportiumParser(IDocumentLoader documentLoader, SportiumSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override void ParseTitle()
        {
            var documentNode = this.Context.Document.DocumentNode;
            var productEssential = documentNode.SelectSingleNode(".//div[@class='product-essential']");
            var productName = productEssential.SelectSingleNode(".//div[@class='product-name']");
            var title = productName.SelectSingleNode(".//h1").InnerText;
            
            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var imageNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='product-essential']")
                .SelectSingleNode(".//div[@class='product-image-gallery']")
                .SelectSingleNode(".//img[@class='gallery-image visible']");
                
            var extractedValue = imageNode.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var documentNode = this.Context.Document.DocumentNode;
            var productEssential = documentNode.SelectSingleNode(".//div[@class='product-essential']");
            var priceBox = productEssential.SelectSingleNode(".//div[@class='price-box']");
            var prices = priceBox.SelectNodes(".//span[@class='price']");
            var productPrice = prices.FirstOrDefault(x => x.Id.Contains("product-price"));

            if (productPrice == null && prices.Count == 1)
            {
                productPrice = prices.First();
            }

            var nodeValue = productPrice?.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}