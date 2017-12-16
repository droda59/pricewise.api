using System;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class MECParser : BaseHtmlParser
    {
        public MECParser(IDocumentLoader documentLoader, MECSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            //.//span[contains(@class, 'a-button')]
            var title = doc.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'product-layout')]")
                .SelectNodes(".//h1[@class='product__name']")
                .First()
                .InnerText;

            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'product-layout')]")
                .SelectSingleNode(".//div[contains(@class, 'carousel')]")
                .SelectNodes(".//img[contains(@class, 'fluid-image__content')]")
                .First();
                
            var extractedValue = imageNode.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var price = doc.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'product-layout')]")
                .SelectSingleNode(".//ul[@class='price-group']")
                .SelectSingleNode(".//li[@class='price']")
                .SelectSingleNode(".//span[@itemprop='price']")
                .InnerText;

            var decimalValue = price.ExtractDecimal();

            return decimalValue;
        }
    }
}