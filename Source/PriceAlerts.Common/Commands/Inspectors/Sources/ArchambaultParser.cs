using System;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    internal class ArchambaultParser : BaseHtmlParser
    {
        public ArchambaultParser(IDocumentLoader documentLoader, ArchambaultSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product-info']")
                .SelectSingleNode(".//h1");

            if (titleNode == null)
            {
                titleNode = doc.DocumentNode
                    .SelectSingleNode(".//div[@class='main-content']")
                    .SelectSingleNode(".//h1");
            }

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

            if (priceNodes == null || !priceNodes.Any())
            {
                priceNodes = doc.DocumentNode
                    .SelectNodes(".//p[@class='listed-price']/text()");
            }

            var sb  = new StringBuilder();
            foreach(var node in priceNodes)
            {
                sb.Append(node.InnerText);
            }

            var content = sb.ToString();
            var decimalValue = content.ExtractDecimal();

            return decimalValue;
        }

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var keywordsNode = doc.DocumentNode.SelectSingleNode("//meta[@name='keywords']");
            var keywordsValue = keywordsNode.Attributes["content"].Value;

            var firstKeyword = keywordsValue.Split(',').First();
            if (ISBNHelper.ISBN13Expression.IsMatch(firstKeyword))
            {
                return ISBNHelper.ISBN13Expression.Match(firstKeyword).Value;
            }

            return string.Empty;
        }
    }
}
