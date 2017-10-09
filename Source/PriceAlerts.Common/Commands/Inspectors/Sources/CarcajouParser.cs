using System;
using HtmlAgilityPack;
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

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var descriptionNodes = doc.GetElementbyId("desc_top").SelectNodes(".//span[@class='desc_debut']");
            foreach (var descriptionNode in descriptionNodes)
            {
                if (descriptionNode.InnerText.Contains("ISBN"))
                {
                    var isbnNodeValue = descriptionNode.NextSibling.InnerText;
                    if (ISBNHelper.ISBN13Expression.IsMatch(isbnNodeValue))
                    {
                        return ISBNHelper.ISBN13Expression.Match(isbnNodeValue).Value;
                    }
                }
            }

            return string.Empty;
        }
    }
}
