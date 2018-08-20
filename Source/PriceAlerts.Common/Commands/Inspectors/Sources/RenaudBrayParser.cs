using System;
using System.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class RenaudBrayParser : BaseHtmlParser
    {
        public RenaudBrayParser(IDocumentLoader documentLoader, RenaudBraySource source)
            : base(documentLoader, source)
        {
        }

        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode.SelectSingleNode("//h1[@class='lblTitle']");

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
            var priceNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//*[contains(@class, 'lblPrice_adv2')]");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }

        protected override void ParseProductIdentifier()
        {
            var isbnMetaNode = this.Context.Document.DocumentNode.SelectSingleNode("//meta[@property='og:isbn']");
            if (isbnMetaNode != null)
            {
                var isbnMetaValue = isbnMetaNode.Attributes["content"].Value;
                if (ISBNHelper.ISBN13Expression.IsMatch(isbnMetaValue))
                {
                    this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN13Expression.Match(isbnMetaValue).Value;
                }
            }

            var isbnSectionNode = this.Context.Document.DocumentNode
                .SelectSingleNode("//table[@class='tblDetails_adv2']")
                .SelectSingleNode(".//tr[contains(@id, 'ISBN')]");

            if (isbnSectionNode != null)
            {
                var isbnNode = isbnSectionNode.SelectSingleNode(".//span[contains(@id, 'ISBNInfo')]");
                var isbn13Value = isbnNode.InnerText.Split(' ').First();
                if (ISBNHelper.ISBN13Expression.IsMatch(isbn13Value))
                {
                    this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN13Expression.Match(isbn13Value).Value;
                }

                var isbn10Value = isbnNode.InnerText.Split(' ').Last().Replace("(", string.Empty).Replace(")", string.Empty);
                if (ISBNHelper.ISBN10Expression.IsMatch(isbn10Value))
                {
                    this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN10Expression.Match(isbn10Value).Value;
                }
            }
        }
    }
}
