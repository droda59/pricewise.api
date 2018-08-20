using System;
using System.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class BestBuyParser : BaseHtmlParser
    {
        public BestBuyParser(IDocumentLoader documentLoader, BestBuySource source)
            : base(documentLoader, source)
        {
        }
        
        protected override void ParseTitle()
        {
            var titleNodes = this.Context.Document.DocumentNode
                .SelectSingleNode(".//h1[@class='product-title']")
                .SelectNodes(".//span");

            var concatedTitle = string.Join(" ", titleNodes.Select(x => x.InnerText));

            var extractedValue = concatedTitle.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var nodeValue = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='gallery-image-container']")
                .SelectSingleNode(".//img");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'price-wrapper')]")
                .SelectSingleNode(".//div[contains(@class, 'prodprice')]")
                .SelectSingleNode(".//span[@class='amount']");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}
