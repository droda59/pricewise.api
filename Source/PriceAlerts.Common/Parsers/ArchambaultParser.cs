using System;
using System.Text;
using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers
{
    internal class ArchambaultParser : BaseParser, IParser
    {
        public ArchambaultParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.archambault.ca/"))
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product-info']")
                .SelectSingleNode(".//h1");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode.SelectSingleNode("//link[@rel='image_src']");
                
            var extractedValue = imageNode.Attributes["href"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNodes = doc.DocumentNode
                .SelectNodes(".//p[@class='our-price']/text()");

            var sb  = new StringBuilder();
            foreach(var node in priceNodes)
            {
                sb.Append(node.InnerText);
            }

            var content = sb.ToString();
            var extractedValue = content.ExtractNumber();
            var decimalValue = Convert.ToDecimal(extractedValue);

            return decimalValue;
        }
    }
}
