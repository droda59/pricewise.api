using System;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class CarcajouParser : BaseHtmlParser
    {
        public CarcajouParser(IDocumentLoader documentLoader, CarcajouSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.GetElementbyId("desc_title");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var nodeValue = this.Context.Document.GetElementbyId("image_produit")
                .SelectSingleNode(".//img");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//span[contains(@class, 'desc_price')]")
                .SelectSingleNode(".//span[contains(@class, 'price')]");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }

        protected override void ParseProductIdentifier()
        {
            var descriptionNodes = this.Context.Document.GetElementbyId("desc_top").SelectNodes(".//span[@class='desc_debut']");
            foreach (var descriptionNode in descriptionNodes)
            {
                if (descriptionNode.InnerText.Contains("ISBN"))
                {
                    var isbnNodeValue = descriptionNode.NextSibling.InnerText;
                    if (ISBNHelper.ISBN13Expression.IsMatch(isbnNodeValue))
                    {
                        this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN13Expression.Match(isbnNodeValue).Value;
                    }
                }
            }
        }
    }
}
