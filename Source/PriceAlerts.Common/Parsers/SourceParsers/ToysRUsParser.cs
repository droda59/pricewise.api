using System;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    public class ToysRUsParser : BaseParser, IParser
    {
        public ToysRUsParser(IDocumentLoader documentLoader)
            : base(documentLoader, new ToysRUsSource())
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");

            var extractedValue = titleNode.Attributes["content"].Value.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                
            var extractedValue = imageNode.Attributes["content"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc
                .GetElementbyId("price")
                .SelectSingleNode(".//dl[@class='price']")
                .SelectSingleNode(".//dd[@class='ours']");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            return decimalValue;
        }
    }
}
