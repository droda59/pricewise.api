using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    internal class RenaudBrayParser : BaseParser, IParser
    {
        private readonly Regex _isbn13Expression;
        private readonly Regex _isbn10Expression;

        public RenaudBrayParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.renaud-bray.com/"))
        {
            this._isbn13Expression = new Regex(@"[0-9]{13}", RegexOptions.Compiled);
            this._isbn10Expression = new Regex(@"[0-9]{10}", RegexOptions.Compiled);
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='lblTitle']");

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
            var priceNode = doc.DocumentNode
                .SelectSingleNode(".//*[contains(@class, 'lblPrice_adv2')]");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            return decimalValue;
        }

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var isbnSectionNode = doc.DocumentNode
                .SelectSingleNode("//table[@class='tblDetails_adv2']")
                .SelectSingleNode(".//tr[contains(@id, 'ISBN')]");

            if (isbnSectionNode != null)
            {
                var isbnNode = isbnSectionNode.SelectSingleNode(".//span[contains(@id, 'ISBNInfo')]");
                var isbn13Value = isbnNode.InnerText.Split(' ').First();
                if (this._isbn13Expression.IsMatch(isbn13Value))
                {
                    return this._isbn13Expression.Match(isbn13Value).Value;
                }

                var isbn10Value = isbnNode.InnerText.Split(' ').Last().Replace("(", string.Empty).Replace(")", string.Empty);
                if (this._isbn10Expression.IsMatch(isbn10Value))
                {
                    return this._isbn10Expression.Match(isbn10Value).Value;
                }
            }

            return string.Empty;
        }
    }
}
