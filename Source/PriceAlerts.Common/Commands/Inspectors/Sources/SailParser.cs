using System;
using System.Linq;
using HtmlAgilityPack;
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
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var title = doc.DocumentNode.SelectSingleNode(".//div[@class='product-essential']")
                                .SelectSingleNode(".//div[@class='product-shop']")
                                .SelectSingleNode(".//div[@class='product-name']")
                                .SelectSingleNode(".//span")
                                .InnerText;

            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product-essential']")
                .SelectSingleNode(".//div[@class='product-image-gallery']")
                .SelectSingleNode(".//img[@class='gallery-image visible']");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
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

            return decimalValue;
        }
    }
}