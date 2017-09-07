using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    internal class LeonParser : BaseHtmlParser
    {
        public LeonParser(IDocumentLoader documentLoader, LeonSource source)
            : base(documentLoader, source)
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
            var imageNode = doc.GetElementbyId("new-zoom").SelectSingleNode("//a[@id='zoom']");
                
            var extractedValue = imageNode.Attributes["href"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:price:amount']");

            var decimalValue = priceNode.Attributes["content"].Value.ExtractDecimal();

            return decimalValue;
        }
    }
}
