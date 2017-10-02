using System;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    internal class LegoParser : BaseHtmlParser
    {
        public LegoParser(IDocumentLoader documentLoader, LegoSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='overview__info']")
                .SelectSingleNode(".//h1[contains(@class, 'overview__name')]");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product']")
                .SelectSingleNode(".//div[@class='product-features']")
                .SelectSingleNode(".//div[@class='product-features__img-holder']")
                .SelectNodes(".//img").First();
                
            var extractedValue = imageNode.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceContentNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='overview__info']")
                .SelectSingleNode(".//div[@class='product-price']")
                .SelectSingleNode(".//span[@class='product-price__list-price']");
            
            if (priceContentNode == null)
            {
                priceContentNode = doc.DocumentNode
                    .SelectSingleNode(".//div[@class='overview__info']")
                    .SelectSingleNode(".//div[@class='product-price']")
                    .SelectSingleNode(".//div[@class='product-price__sale']//span");
            }

            var decimalValue = priceContentNode.InnerText.ExtractDecimal();

            return decimalValue;
        }
    }
}
