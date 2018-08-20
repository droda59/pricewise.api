using System;
using System.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class BoardGameBlissParser : BaseHtmlParser
    {
        public BoardGameBlissParser(IDocumentLoader documentLoader, BoardGameBlissSource source)
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
            // This site is doing some Hacky McHack stuff in javascript to modify the dom.
            // This seems to prevent us from fetching directly the price-preview node with its id on some pages.
            // Also, you might not see this node by inspecting some pages because the javascript might tamper with it.
            var priceNode = this.Context.Document
                .GetElementbyId("product")
                .SelectSingleNode(".//div[@class='purchase']")
                .SelectSingleNode(".//h2[@id='price-preview']");

            var nodeValue = priceNode.InnerText;
            
            // During sales, the InnerText value might look like the following : $ 45.99 CAD$40.00 CAD
            // we can safely assume the discounted price appears last.
            var decimalValue = nodeValue.Split("$").Last().ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}