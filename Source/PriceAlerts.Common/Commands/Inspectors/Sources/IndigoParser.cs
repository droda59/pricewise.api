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

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product_description-container']")
                .SelectSingleNode(".//h1[contains(@class, 'item-page__main-title')]");

            var titleAttribute = titleNode.Attributes["title"].Value;

            var extractedValue = titleAttribute.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product-image-container']")
                .SelectSingleNode(".//img[contains(@class, 'product-image__image')]");

            return nodeValue.Attributes["src"].Value;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var itemPriceNode = doc.DocumentNode.SelectSingleNode(".//div[@class='item-price']");

            string nodeValue;
            var priceContainer = itemPriceNode.SelectSingleNode(".//div[@class='item-price__container']");
            if (priceContainer != null)
            {
                IEnumerable<HtmlNode> priceNodes;
                priceContainer = priceContainer.SelectSingleNode(".//span[@class='item-price__price-amount']");

                if (priceContainer.SelectNodes("./span") != null)
                {
                    priceNodes = priceContainer
                        .SelectNodes("./span")
                        .SelectMany(x => x.ChildNodes)
                        .Where(x => x.NodeType == HtmlNodeType.Text);
                }
                else
                {
                    priceNodes = priceContainer.ChildNodes
                        .Where(x => x.NodeType == HtmlNodeType.Text);
                }

                nodeValue = string.Join(" ", priceNodes.Select(x => x.InnerText));
            }
            else 
            {
                priceContainer = itemPriceNode.SelectSingleNode(".//p[@class='item-price__normal']");
                nodeValue = priceContainer.InnerText;
            }

            var decimalValue = nodeValue.ExtractDecimal();

            return decimalValue;
        }

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var isbnRootNode = doc.DocumentNode.SelectSingleNode(".//div[@class='item-page__isbn-items']");
            if (isbnRootNode != null)
            {
                var isbnNodes = isbnRootNode.SelectNodes(".//span[@class='item-page__spec-value']");
                foreach (var isbnNode in isbnNodes)
                {
                    if (ISBNHelper.ISBN13Expression.IsMatch(isbnNode.InnerText))
                    {
                        return ISBNHelper.ISBN13Expression.Match(isbnNode.InnerText).Value;
                    }
                }
                
                foreach (var isbnNode in isbnNodes)
                {
                    if (ISBNHelper.ISBN10Expression.IsMatch(isbnNode.InnerText))
                    {
                        return ISBNHelper.ISBN10Expression.Match(isbnNode.InnerText).Value;
                    }
                }
            }

            var upcNodes = doc.DocumentNode.SelectNodes(".//span[@class='item-page__spec-label']");
            foreach (var upcNode in upcNodes)
            {
                if (upcNode.InnerText.Contains("UPC"))
                {
                    return upcNode.NextSibling.InnerText;
                }
            }

            return string.Empty;
        }
    }
}
