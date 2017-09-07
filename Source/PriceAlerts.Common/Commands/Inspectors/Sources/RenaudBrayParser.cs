using System;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    internal class RenaudBrayParser : BaseHtmlParser
    {
        public RenaudBrayParser(IDocumentLoader documentLoader, RenaudBraySource source)
            : base(documentLoader, source)
        {
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
            var isbnMetaNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:isbn']");
            if (isbnMetaNode != null)
            {
                var isbnMetaValue = isbnMetaNode.Attributes["content"].Value;
                if (ISBNHelper.ISBN13Expression.IsMatch(isbnMetaValue))
                {
                    return ISBNHelper.ISBN13Expression.Match(isbnMetaValue).Value;
                }
            }

            var isbnSectionNode = doc.DocumentNode
                .SelectSingleNode("//table[@class='tblDetails_adv2']")
                .SelectSingleNode(".//tr[contains(@id, 'ISBN')]");

            if (isbnSectionNode != null)
            {
                var isbnNode = isbnSectionNode.SelectSingleNode(".//span[contains(@id, 'ISBNInfo')]");
                var isbn13Value = isbnNode.InnerText.Split(' ').First();
                if (ISBNHelper.ISBN13Expression.IsMatch(isbn13Value))
                {
                    return ISBNHelper.ISBN13Expression.Match(isbn13Value).Value;
                }

                var isbn10Value = isbnNode.InnerText.Split(' ').Last().Replace("(", string.Empty).Replace(")", string.Empty);
                if (ISBNHelper.ISBN10Expression.IsMatch(isbn10Value))
                {
                    return ISBNHelper.ISBN10Expression.Match(isbn10Value).Value;
                }
            }

            // var upcMetaNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:upc']");
            // if (upcMetaNode != null)
            // {
            //     return isbnMetaNode.Attributes["content"].Value;
            // }

            return string.Empty;
        }
    }
}
