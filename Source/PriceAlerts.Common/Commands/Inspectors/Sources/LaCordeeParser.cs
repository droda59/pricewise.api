using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
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
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var itemDetails = documentNode.SelectSingleNode(".//div[@class='item-details block']");
            var textNode = itemDetails.SelectSingleNode(".//h1");
            var title = textNode.InnerText;
            
            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productImage = documentNode.SelectSingleNode(".//p[@class='product-image']");
            var imageNode = productImage.SelectSingleNode(".//img");
                
            var extractedValue = imageNode.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceBox = doc.DocumentNode.SelectSingleNode(".//div[@class='price-box']");
            var specialPrice = priceBox.SelectSingleNode(".//p[@class='special-price']");

            var priceNode = specialPrice == null ? 
                priceBox.SelectSingleNode(".//span[@class='price']") : 
                specialPrice.SelectSingleNode(".//span[@class='price']");

            var decimalValue = priceNode.InnerText.ExtractDecimal();

            return decimalValue;
        }
    }
}