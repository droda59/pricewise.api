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
            var titleNode = doc.DocumentNode.SelectSingleNode(".//h1[contains(@class, 'product-description__title') and contains(@class, 'title')]");
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
            var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                
            var extractedValue = imageNode.Attributes["content"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNodes = doc.DocumentNode
                .SelectNodes(".//div[contains(@class, 'product-price__total')]")
                .Where(x => !string.IsNullOrEmpty(x.InnerText))
                .ToList();

            if (priceNodes.Any())
            {
                var content = priceNodes.First().InnerText;
                var decimalValue = content.ExtractDecimal();

                return decimalValue;
            }

            return 0;
        }

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var propertiesNode = doc.GetElementbyId("outerDivProperties");
            var properties = propertiesNode.SelectNodes(".//div[@class='block-description__row']");
            foreach (var property in properties)
            {
                var category = property.SelectSingleNode(".//div[@class='block-description__row-category']");
                var value = property.SelectSingleNode(".//div[@class='block-description__row-value']");
                if (category.InnerText == "ISBN" && ISBNHelper.ISBN13Expression.IsMatch(value.InnerText))
                {
                    return ISBNHelper.ISBN13Expression.Match(value.InnerText).Value;
                }
            }

            return string.Empty;
        }
    }
}
