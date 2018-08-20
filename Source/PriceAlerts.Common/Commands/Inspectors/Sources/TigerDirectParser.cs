using System;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class TigerDirectParser : BaseHtmlParser
    {
        public TigerDirectParser(IDocumentLoader documentLoader, TigerDirectSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='prodName']")
                .SelectSingleNode(".//h1");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var nodeValue = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='prdImg']")
                .SelectSingleNode(".//td[@class='previewImgHolder']")
                .SelectSingleNode(".//img");
                
            var extractedValue = nodeValue.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceContent = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@itemprop='offers']")
                .SelectSingleNode(".//meta[@itemprop='price']")
                .Attributes["content"].Value;

            var decimalValue = priceContent.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}
