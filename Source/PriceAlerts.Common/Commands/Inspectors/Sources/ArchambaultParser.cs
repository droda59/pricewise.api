using System;
using System.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class ArchambaultParser : BaseHtmlParser
    {
        public ArchambaultParser(IDocumentLoader documentLoader, ArchambaultSource source)
            : base(documentLoader, source)
        {
        }

        protected override void ParseTitle()
        {
            var doc = this.Context.Document;
            var titleNode = doc.DocumentNode.SelectSingleNode(".//h1[contains(@class, 'product-description__title') and contains(@class, 'title')]");
            if (titleNode == null)
            {
                titleNode = doc.DocumentNode
                    .SelectSingleNode(".//div[@class='main-content']")
                    .SelectSingleNode(".//h1");
            }

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var imageNode = this.Context.Document.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                
            var extractedValue = imageNode.Attributes["content"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceNodes = this.Context.Document.DocumentNode
                .SelectNodes(".//div[contains(@class, 'product-price__total')]")
                .Where(x => !string.IsNullOrEmpty(x.InnerText))
                .ToList();

            if (priceNodes.Any())
            {
                var content = priceNodes.First().InnerText;
                var decimalValue = content.ExtractDecimal();

                this.Context.SitePriceInfo.Price = decimalValue;
            }

            this.Context.SitePriceInfo.Price =  0;
        }

        protected override void ParseProductIdentifier()
        {
            var propertiesNode = this.Context.Document.GetElementbyId("outerDivProperties");
            var properties = propertiesNode.SelectNodes(".//div[@class='block-description__row']");
            foreach (var property in properties)
            {
                var category = property.SelectSingleNode(".//div[@class='block-description__row-category']");
                var value = property.SelectSingleNode(".//div[@class='block-description__row-value']");
                if (category.InnerText == "ISBN" && ISBNHelper.ISBN13Expression.IsMatch(value.InnerText))
                {
                    this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN13Expression.Match(value.InnerText).Value;
                }
            }
        }
    }
}
