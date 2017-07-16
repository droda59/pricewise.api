using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    internal class CarcajouParser : BaseParser, IParser
    {
        private readonly Regex _isbn13Expression;
        
        public CarcajouParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.librairiecarcajou.com/"))
        {
            this._isbn13Expression = new Regex(@"[0-9]{13}", RegexOptions.Compiled);
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
                    if (this._isbn13Expression.IsMatch(isbnNodeValue))
                    {
                        return this._isbn13Expression.Match(isbnNodeValue).Value;
                    }
                }
            }

            return string.Empty;
        }
    }
}
