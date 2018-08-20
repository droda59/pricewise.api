using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class StaplesParser : BaseHtmlParser
    {
        public StaplesParser(IDocumentLoader documentLoader, StaplesSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='product-microformat']")
                .SelectSingleNode(".//span[@itemprop='name']");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

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
            var priceContent = this.Context.Document.DocumentNode
                .SelectSingleNode(".//*[@class='SEOFinalPrice']")
                .InnerText;

            var decimalValue = priceContent.ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }

        // protected override void ParseProductIdentifier()
        // {
        //     var modelNumberNode = doc.DocumentNode
        //         .SelectSingleNode("//div[contains(@class,'product-title-section')]")
        //         .SelectSingleNode(".//ul[contains(@class, 'item-subtitle')]//span[@ng-bind='product.metadata.mfpartnumber']");

        //     if (modelNumberNode != null)
        //     {
        //         return modelNumberNode.InnerText;
        //     }

        //     return string.Empty;
        // }
    }
}
