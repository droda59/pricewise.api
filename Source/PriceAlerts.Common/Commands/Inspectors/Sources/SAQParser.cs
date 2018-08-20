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

        protected override void ParseTitle()
        {
            var titleNode = this.Context.Document.DocumentNode.SelectSingleNode("//h1[@class='product-description-title']");
            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var containerNode = this.Context.Document.DocumentNode.SelectSingleNode("//div[@class='product-description-image']");
            var sourceRegex = new Regex("src\\s*=\\s*\"(.+?)\"");
            var match = sourceRegex.Match(containerNode.OuterHtml);
            var extractedValue = match.Value.Substring(7, match.Value.Length -  8);

            this.Context.SitePriceInfo.ImageUrl =  "http://" + extractedValue;
        }

        protected override void ParsePrice()
        {
            var priceNode = this.Context.Document.DocumentNode.SelectSingleNode("//p[@class='price']");

            if(priceNode == null){
                priceNode = this.Context.Document.DocumentNode.SelectSingleNode("//p[@class='price price-rebate']");
            }

            var decimalValue = priceNode.InnerText.Split(':')[1].Replace("*", string.Empty).ExtractDecimal();

            this.Context.SitePriceInfo.Price = decimalValue;
        }
//
//        protected override void ParseProductIdentifier()
//        {
//            var productIdNode = doc.DocumentNode.SelectSingleNode("//div[@class='product-description-row2']");
//            var text = productIdNode.InnerText.Split(':')[2].Replace("\r\n", String.Empty).Trim();
//
//            return text;
//        }
    }
}
