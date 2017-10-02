using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    internal class IkeaParser : BaseHtmlParser
    {
        public IkeaParser(IDocumentLoader documentLoader, IkeaSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@name='title']");

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
            var priceNode = doc.DocumentNode.SelectSingleNode("//meta[@name='price']");

            var decimalValue = priceNode.Attributes["content"].Value.ExtractDecimal();

            return decimalValue;
        }
    }
}
