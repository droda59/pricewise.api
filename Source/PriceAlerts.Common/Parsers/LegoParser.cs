using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers
{
    internal class LegoParser : BaseParser, IParser
    {
        public LegoParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("https://shop.lego.com/"))
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='overview__info']")
                .SelectSingleNode(".//h1[@class='overview__name']");

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
            var priceContent = doc.DocumentNode
                .SelectSingleNode(".//div[@class='overview__info']")
                .SelectSingleNode(".//div[@class='product-price']")
                .SelectSingleNode(".//span[@class='product-price__list-price']")
                .InnerText;

            var extractedValue = priceContent.ExtractNumber();
            var decimalValue = Convert.ToDecimal(extractedValue);

            return decimalValue;
        }
    }
}
