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
            var documentNode = doc.DocumentNode;
            var productLayout = documentNode.SelectSingleNode(".//div[contains(@class, 'product-layout')]");
            var carousel = productLayout.SelectSingleNode(".//div[contains(@class, 'carousel')]");
            var images = carousel.SelectNodes(".//img[contains(@class, 'fluid-image__content')]");
            var imageNode = images == null ? carousel.SelectSingleNode(".//img") : images.First();
                
            var extractedValue = imageNode.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productLayout = documentNode.SelectSingleNode(".//div[contains(@class, 'product-layout')]");
            var priceGroup = productLayout.SelectSingleNode(".//ul[@class='price-group']");
            var priceNode = priceGroup.SelectSingleNode(".//li[@class='price']");

            var lowprice = priceNode.SelectSingleNode(".//span[@itemprop='lowPrice']");
            var price = lowprice != null ? lowprice.InnerText : priceNode.LastChild.InnerText;

            var decimalValue = price.ExtractDecimal();

            return decimalValue;
        }
    }
}