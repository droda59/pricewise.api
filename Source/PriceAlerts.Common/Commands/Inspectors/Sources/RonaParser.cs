﻿using System;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class RonaParser : BaseHtmlParser
    {
        public RonaParser(IDocumentLoader documentLoader, RonaSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");

            var extractedValue = titleNode.Attributes["content"].Value.Replace(Environment.NewLine, string.Empty).Trim();

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
            var priceContentNode = doc.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'productDetails')]")
                .SelectSingleNode(".//span[contains(@class, 'product_price')]")
                .SelectSingleNode(".//span[contains(@class, 'product_price_amount')]");

            var dollarsValue = priceContentNode.SelectSingleNode(".//span[contains(@class, 'integer')]");
            var centsValue = priceContentNode.SelectSingleNode(".//span[contains(@class, 'decimal')]");

            var decimalDollarsValue = dollarsValue?.InnerText.ExtractDecimal() ?? 0m;
            var decimalCentsValue = centsValue?.InnerText.ExtractDecimal() ?? 0m;

            return decimalDollarsValue + (decimalCentsValue / 100);
        }
    }
}