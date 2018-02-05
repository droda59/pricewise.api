using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class SportsExpertsParser : BaseHtmlParser
    {
        public SportsExpertsParser(IDocumentLoader documentLoader, SportsExpertsSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var title = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product-detail']")
                .SelectSingleNode(".//span[@class='product-detail-name']")
                .InnerText;
            
            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode
                .SelectSingleNode(".//div[@class='product-detail']")
                .SelectSingleNode(".//div[@class='product-details-media']")
                .SelectSingleNode(".//img[@class='img-fluid']");
                
            var extractedValue = imageNode.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var productDetail = doc.DocumentNode.SelectSingleNode(".//div[@class='product-detail']");
            var regex = new Regex("(?<=listprice\":\")(\\$?\\d+[\\.,\\,]\\d+)");
            var captures = regex.Match(productDetail.OuterHtml).Captures;
            var price = captures.First();

            var decimalValue = price.Value.ExtractDecimal();

            return decimalValue;
        }
    }
}