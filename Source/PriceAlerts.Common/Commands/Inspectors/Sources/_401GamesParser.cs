using System;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class _401GamesParser : BaseHtmlParser
    {
        public _401GamesParser(IDocumentLoader documentLoader, _401GamesSource source)
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
            var imageNode = this.Context.Document.DocumentNode.SelectSingleNode("//meta[@property='og:price:amount']");
                
            var extractedValue = imageNode.Attributes["content"].Value;

            this.Context.SitePriceInfo.Price = extractedValue.ExtractDecimal();
        }
    }
}