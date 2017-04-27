using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers
{
    internal class StaplesParser : BaseParser, IParser
    {
        public StaplesParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.staples.ca/"))
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
                .SelectSingleNode(".//div[@class='SEOFinalPrice']")
                .InnerText;

            var extractedValue = this.ExtractNumber(priceContent);
            var decimalValue = Convert.ToDecimal(extractedValue);

            return decimalValue;
        }
    }
}
