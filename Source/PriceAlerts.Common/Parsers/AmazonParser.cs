using System;
using System.Linq;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers
{
    internal class AmazonParser : BaseParser, IParser
    {
        public AmazonParser(IHtmlLoader htmlLoader)
            : base(htmlLoader)
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var nodeValue = doc.GetElementbyId("productTitle").InnerText;
            var extractedValue = nodeValue.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.GetElementbyId("landingImage");
            var extractedValue = nodeValue.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc.GetElementbyId("priceblock_ourprice");
            if (priceNode == null)
            {
                priceNode = doc.GetElementbyId("priceblock_dealprice");
            }

            var nodeValue = priceNode.InnerText;
            var extractedValue = this.ExtractNumber(nodeValue);
            var decimalValue = Convert.ToDecimal(extractedValue);

            return decimalValue;
        }

        private string ExtractNumber(string original)
        {
            return new string(original.Where(c => Char.IsDigit(c) || Char.IsPunctuation(c)).ToArray());
        }
    }
}
