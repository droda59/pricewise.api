using System;
using System.Linq;

using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class LegoParser : BaseHtmlParser
    {
        public LegoParser(IDocumentLoader documentLoader, LegoSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='overview__info']")
                .SelectSingleNode(".//h1[contains(@class, 'overview__name')]");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var imageNode = this.Context.Document.DocumentNode
                .SelectSingleNode(".//div[@class='product']")
                .SelectSingleNode(".//div[@class='product-features']")
                .SelectSingleNode(".//div[@class='product-features__img-holder']")
                .SelectNodes(".//img").First();
                
            var extractedValue = imageNode.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var doc = this.Context.Document;
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

            this.Context.SitePriceInfo.Price = decimalValue;
        }
    }
}
