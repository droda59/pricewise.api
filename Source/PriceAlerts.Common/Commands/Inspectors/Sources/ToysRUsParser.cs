using System;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class ToysRUsParser : BaseHtmlParser
    {
        public ToysRUsParser(IDocumentLoader documentLoader, ToysRUsSource source)
            : base(documentLoader, source)
        {
        }

        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode.SelectSingleNode("//meta[@property='og:title']");

            var extractedValue = titleNode.Attributes["content"].Value.Replace(Environment.NewLine, string.Empty).Trim();

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
            var priceNode = this.Context.Document
                .GetElementbyId("price")
                .SelectSingleNode(".//dl[@class='price']")
                .SelectSingleNode(".//dd[@class='ours']");

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}
