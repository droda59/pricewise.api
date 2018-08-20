using System;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class LeonParser : BaseHtmlParser
    {
        public LeonParser(IDocumentLoader documentLoader, LeonSource source)
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
            var imageNode = this.Context.Document.GetElementbyId("new-zoom").SelectSingleNode("//a[@id='zoom']");
                
            var extractedValue = imageNode.Attributes["href"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceNode = this.Context.Document.DocumentNode.SelectSingleNode("//meta[@property='og:price:amount']");

            var decimalValue = priceNode.Attributes["content"].Value.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}
