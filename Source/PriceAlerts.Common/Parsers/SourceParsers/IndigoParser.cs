using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    internal class IndigoParser : BaseParser, IParser
    {
        public IndigoParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("https://www.chapters.indigo.ca/"))
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
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var itemPriceNode = doc.DocumentNode.SelectSingleNode(".//div[@class='item-price']");

            string nodeValue;
            var priceContainer = itemPriceNode.SelectSingleNode(".//div[@class='item-price__container']");
            if (priceContainer != null)
            {
                var priceNodes = Enumerable.Empty<HtmlNode>();
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
    }
}
