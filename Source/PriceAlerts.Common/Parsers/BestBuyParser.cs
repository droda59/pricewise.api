using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers
{
    internal class BestBuyParser : BaseParser, IParser
    {
        public BestBuyParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.bestbuy.ca/"))
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNodes = doc.DocumentNode
                .SelectSingleNode(".//h1[@class='product-title']")
                .SelectNodes(".//span");

            var concatedTitle = string.Join(" ", titleNodes.Select(x => x.InnerText));

            var extractedValue = concatedTitle.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.DocumentNode
                .SelectSingleNode(".//div[@class='gallery-image-container']")
                .SelectSingleNode(".//img");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'price-wrapper')]")
                .SelectSingleNode(".//div[contains(@class, 'prodprice')]")
                .SelectSingleNode(".//span[@class='amount']");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            return decimalValue;
        }
    }
}
