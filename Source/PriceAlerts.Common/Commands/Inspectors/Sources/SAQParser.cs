using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class SAQParser : BaseHtmlParser
    {
        public SAQParser(IDocumentLoader documentLoader, SAQSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='product-description-title']");
            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var containerNode = doc.DocumentNode.SelectSingleNode("//div[@class='product-description-image']");
            var sourceRegex = new Regex("src\\s*=\\s*\"(.+?)\"");
            var match = sourceRegex.Match(containerNode.OuterHtml);
            var extractedValue = match.Value.Substring(7, match.Value.Length -  8);

            return "http://" + extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc.DocumentNode.SelectSingleNode("//p[@class='price']");

            if(priceNode == null){
                priceNode = doc.DocumentNode.SelectSingleNode("//p[@class='price price-rebate']");
            }

            var decimalValue = priceNode.InnerText.Split(':')[1].Replace("*", string.Empty).ExtractDecimal();

            return decimalValue;
        }
//
//        protected override string GetProductIdentifier(HtmlDocument doc)
//        {
//            var productIdNode = doc.DocumentNode.SelectSingleNode("//div[@class='product-description-row2']");
//            var text = productIdNode.InnerText.Split(':')[2].Replace("\r\n", String.Empty).Trim();
//
//            return text;
//        }
    }
}
