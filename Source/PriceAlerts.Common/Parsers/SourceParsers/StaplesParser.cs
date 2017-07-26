using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    internal class StaplesParser : BaseParser, IParser
    {
        public StaplesParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("https://www.staples.ca/"))
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product-microformat']")
                .SelectSingleNode(".//span[@itemprop='name']");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                
            var extractedValue = imageNode.Attributes["content"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceContent = doc.DocumentNode
                .SelectSingleNode(".//*[@class='SEOFinalPrice']")
                .InnerText;

            var decimalValue = priceContent.ExtractDecimal();

            return decimalValue;
        }

        // protected override string GetProductIdentifier(HtmlDocument doc)
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
