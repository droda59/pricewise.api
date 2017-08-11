using System;
using System.Linq;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    public class MonoPriceParser : BaseParser, IParser
    {
        public MonoPriceParser(IDocumentLoader documentLoader)
            : base(documentLoader, new MonoPriceSource())
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='product-name']");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.GetElementbyId("mono4");
            var extractedValue = nodeValue.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc.DocumentNode.SelectSingleNode("//span[@class='sale-price']");

            var decimalValue = priceNode.InnerText.ExtractDecimal();

            return decimalValue;
        }

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var productIdNode = doc.DocumentNode.SelectSingleNode("//div[@class='product-code']");
            var text = productIdNode.InnerText;

            return text.Split('#')[1].Trim();
        }
    }
}
