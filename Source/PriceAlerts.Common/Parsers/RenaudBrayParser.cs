using System;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers
{
    internal class RenaudBrayParser : BaseParser, IParser
    {
        public RenaudBrayParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.renaud-bray.com/"))
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='lblTitle']");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

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
            var priceNode = doc.DocumentNode
                .SelectSingleNode(".//*[contains(@class, 'lblPrice_adv2')]");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            return decimalValue;
        }
    }
}
