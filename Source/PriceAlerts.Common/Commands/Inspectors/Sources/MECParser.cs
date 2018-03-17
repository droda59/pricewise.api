using System;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class MECParser : BaseHtmlParser
    {
        public MECParser(IDocumentLoader documentLoader, MECSource source)
            : base(documentLoader, source)
        {
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productLayout = documentNode.SelectSingleNode(".//div[contains(@class, 'product-layout')]");
            var productNames = productLayout.SelectNodes(".//h1[@class='product__name']");
            var title = productNames.First().InnerText;

            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var imageMetaNode = documentNode.SelectSingleNode(".//meta[contains(@property, 'og:image')]");
            var extractedValue = imageMetaNode.Attributes["content"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productLayout = documentNode.SelectSingleNode(".//div[contains(@class, 'product-layout')]");
            var priceGroup = productLayout.SelectSingleNode(".//ul[@class='price-group']");
            var priceNode = priceGroup.SelectSingleNode(".//li[@class='price']");

            var lowprice = priceNode.SelectSingleNode(".//span[@itemprop='lowPrice']");
            var price = lowprice != null ? lowprice.InnerText : priceNode.LastChild.InnerText;

            try
            {
                var decimalValue = price.ExtractDecimal();
                return decimalValue;

            }
            catch (Exception)
            {
                // MEC sometimes displays different a range of prices for a single product. We sadly have to way of determining the correct price for the product.
                if (price != null && price.Contains("-"))
                {
                    throw new NotSupportedException("PriceWise was unable to determine the correct price for the selected product.");
                }

                throw;
            }
        }
    }
}