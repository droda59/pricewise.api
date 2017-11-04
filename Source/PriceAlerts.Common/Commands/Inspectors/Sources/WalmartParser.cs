using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class WalmartParser : BaseHtmlParser
    {
        public WalmartParser(IDocumentLoader documentLoader, WalmartSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@id='product-desc']//h1");
            var extractedValue = titleNode.FirstChild.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var image = doc.DocumentNode.SelectSingleNode("//div[@id='product-images']//div[@class='centered-img-wrap']//img");
            var extractedValue = new Uri(this.Source.Domain, image.Attributes["src"].Value);

            return extractedValue.AbsoluteUri;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNodes = doc.DocumentNode.SelectNodes("//div[@class='price-current']");

            string priceContent = "";

            var decimalValue = priceContent.ExtractDecimal();

            return decimalValue;
        }
    }
}
