using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class NcixParser : BaseHtmlParser
    {
        public NcixParser(IDocumentLoader documentLoader, NcixSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='h1description']");
            var extractedValue = titleNode.FirstChild.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var image = doc.DocumentNode.SelectSingleNode("//img[@id='productpicture']");
            var extractedValue = new Uri(this.Source.Domain, image.Attributes["src"].Value);

            return extractedValue.AbsoluteUri;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNodes = doc.DocumentNode
                            .SelectNodes(".//span[@itemprop='price']");

            string priceContent;
            if(priceNodes.Count > 1)
            {
                // Mail-In rebate
                priceContent = priceNodes[1].InnerText;
            }
            else{
                priceContent = priceNodes[0].InnerText;
            }

            var decimalValue = priceContent.ExtractDecimal();

            return decimalValue;
        }
    }
}
