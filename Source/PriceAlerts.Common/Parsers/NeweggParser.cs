using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers
{
    internal class NeweggParser : BaseParser, IParser
    {
        public NeweggParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("https://www.newegg.ca/"))
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc
                .GetElementbyId("grpDescrip_h")
                .SelectSingleNode(".//span");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.DocumentNode
                .SelectSingleNode(".//span[@class='mainSlide']")
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

            var extractedValue = priceContent.ExtractNumber();
            var decimalValue = Convert.ToDecimal(extractedValue);

            return decimalValue;
        }
    }
}
