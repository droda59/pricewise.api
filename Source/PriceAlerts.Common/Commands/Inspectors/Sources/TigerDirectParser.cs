using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    internal class TigerDirectParser : BaseHtmlParser
    {
        public TigerDirectParser(IDocumentLoader documentLoader, TigerDirectSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='prodName']")
                .SelectSingleNode(".//h1");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.DocumentNode
                .SelectSingleNode(".//div[@class='prdImg']")
                .SelectSingleNode(".//td[@class='previewImgHolder']")
                .SelectSingleNode(".//img");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceContent = doc.DocumentNode
                .SelectSingleNode(".//div[@itemprop='offers']")
                .SelectSingleNode(".//meta[@itemprop='price']")
                .Attributes["content"].Value;

            var decimalValue = priceContent.ExtractDecimal();

            return decimalValue;
        }
    }
}
