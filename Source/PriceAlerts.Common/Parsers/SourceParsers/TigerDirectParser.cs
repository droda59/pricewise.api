using System;
using System.Linq;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    public class TigerDirectParser : BaseParser, IParser
    {
        public TigerDirectParser(IDocumentLoader documentLoader)
            : base(documentLoader, new TigerDirectSource())
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
