using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class IndigoParser : BaseHtmlParser
    {
        public IndigoParser(IDocumentLoader documentLoader, IndigoSource source)
            : base(documentLoader, source)
        {
        }

        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='product_description-container']")
                .SelectSingleNode(".//h1[contains(@class, 'item-page__main-title')]");

            var titleAttribute = titleNode.Attributes["title"].Value;

            var extractedValue = titleAttribute.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var nodeValue = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='product-image-container']")
                .SelectSingleNode(".//img[contains(@class, 'product-image__image')]");

            this.Context.SitePriceInfo.ImageUrl = nodeValue.Attributes["src"].Value;
        }

        protected override void ParsePrice()
        {
            var itemPriceNode = this.Context.Document.DocumentNode.SelectSingleNode(".//div[contains(@class, 'item-price')]");

            string nodeValue;
            var priceContainer = itemPriceNode.SelectSingleNode(".//div[contains(@class, 'item-price__container')]");
            if (priceContainer != null)
            {
                var priceNodes = new List<HtmlNode>();
                priceContainer = priceContainer.SelectSingleNode(".//span[contains(@class, 'item-price__price-amount')]");

                if (priceContainer.SelectNodes("./span") != null)
                {
                    priceNodes.AddRange(priceContainer
                        .SelectNodes("./span")
                        .SelectMany(x => x.ChildNodes)
                        .Where(x => x.NodeType == HtmlNodeType.Text));
                }
                
                priceNodes.AddRange(priceContainer.ChildNodes
                    .Where(x => x.NodeType == HtmlNodeType.Text));

                nodeValue = string.Join(" ", priceNodes.Select(x => x.InnerText));
            }
            else 
            {
                priceContainer = itemPriceNode.SelectSingleNode(".//p[contains(@class, 'item-price__normal')]");
                nodeValue = priceContainer.InnerText;
            }

            var decimalValue = nodeValue.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }

        protected override void ParseProductIdentifier()
        {
            var isbnRootNode = this.Context.Document.DocumentNode.SelectSingleNode(".//div[contains(@class, 'item-page__isbn-items')]");
            if (isbnRootNode != null)
            {
                var isbnNodes = isbnRootNode.SelectNodes(".//span[contains(@class, 'item-page__spec-value')]");
                foreach (var isbnNode in isbnNodes)
                {
                    if (ISBNHelper.ISBN13Expression.IsMatch(isbnNode.InnerText))
                    {
                        this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN13Expression.Match(isbnNode.InnerText).Value;
                    }
                }
                
                foreach (var isbnNode in isbnNodes)
                {
                    if (ISBNHelper.ISBN10Expression.IsMatch(isbnNode.InnerText))
                    {
                        this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN10Expression.Match(isbnNode.InnerText).Value;
                    }
                }
            }

            var upcNodes = this.Context.Document.DocumentNode.SelectNodes(".//span[contains(@class, 'item-page__spec-label')]");
            foreach (var upcNode in upcNodes)
            {
                if (upcNode.InnerText.Contains("UPC"))
                {
                    this.Context.SitePriceInfo.ProductIdentifier = upcNode.NextSibling.InnerText;
                }
            }
        }
    }
}
