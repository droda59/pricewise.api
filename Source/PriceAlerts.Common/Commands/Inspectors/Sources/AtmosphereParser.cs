using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class AtmosphereParser : BaseHtmlParser
    {
        public AtmosphereParser(IDocumentLoader documentLoader, AtmosphereSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productDetail = documentNode.SelectSingleNode(".//div[@class='product-detail']");
            var pageHeaderTitle = productDetail.SelectSingleNode(".//h1[@class='global-page-header__title']");
            var title = pageHeaderTitle.InnerText;
            
            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productDetail = documentNode.SelectSingleNode(".//div[@class='product-detail']");
            var imageNode = productDetail.SelectSingleNode(".//img[@class='product-detail__product-img']");

            var extractedValue = imageNode.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var productDetail = doc.DocumentNode.SelectSingleNode(".//div[@class='product-detail']");
            var priceText = productDetail.SelectSingleNode(".//span[@class='product-detail__price-text']");
            var price = priceText.InnerText;

            var decimalValue = price.ExtractDecimal();

            return decimalValue;
        }
    }
}