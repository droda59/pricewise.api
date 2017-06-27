using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    internal class CarcajouParser : BaseParser, IParser
    {
        public CarcajouParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.librairiecarcajou.com/"))
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.GetElementbyId("desc_title");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.GetElementbyId("image_produit")
                .SelectSingleNode(".//img");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc.DocumentNode
                .SelectSingleNode(".//span[contains(@class, 'desc_price')]")
                .SelectSingleNode(".//span[contains(@class, 'price')]");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            return decimalValue;
        }
    }
}
