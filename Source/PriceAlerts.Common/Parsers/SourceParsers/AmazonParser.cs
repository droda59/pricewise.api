using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    internal class AmazonParser : BaseParser, IParser
    {
        private readonly Regex _isbn13Expression;
        private readonly Regex _isbn10Expression;

        public AmazonParser(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("https://www.amazon.ca/"))
        {
            this._isbn13Expression = new Regex(@"[0-9]{13}", RegexOptions.Compiled);
            this._isbn10Expression = new Regex(@"[0-9]{10}", RegexOptions.Compiled);
        }
        
        protected override bool HasRedirectProductUrl(HtmlDocument doc)
        {
            return doc.GetElementbyId("twisterContainer") != null
                && doc.GetElementbyId("priceblock_ourprice") != null
                && doc.GetElementbyId("priceblock_ourprice").InnerText.Contains("-");
        }

        protected override Uri GetRedirectProductUrl(HtmlDocument doc)
        {
            // For size selection in select control
            var optionNodes = doc
                .GetElementbyId("twisterContainer")
                .SelectSingleNode(".//span[@class='a-dropdown-container']")
                .SelectSingleNode(".//select")
                .SelectNodes(".//option");
            
            foreach (var optionNode in optionNodes)
            {
                var optionNodeValue = optionNode.Attributes["value"].Value.Split(',');
                if (optionNodeValue.Length > 1)
                {
                    var productId = optionNodeValue[1];

                    return new Uri(this.Domain, $"/dp/{productId}?th=1&psc=1");
                }
            }

            return null;
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var nodeValue = doc.GetElementbyId("productTitle").InnerText;
            var extractedValue = nodeValue.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var nodeValue = doc.GetElementbyId("main-image-container").SelectNodes(".//img").First();
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

            if (priceNode == null)
            {
                priceNode = doc.GetElementbyId("tmmSwatches");
                if (priceNode != null)
                {
                    var priceNodes = doc.GetElementbyId("tmmSwatches").SelectNodes(".//span[contains(@class, 'a-color-price')]");
                    if (priceNodes == null || !priceNodes.Any())
                    {
                        priceNodes = doc.GetElementbyId("tmmSwatches").SelectNodes(".//span[contains(@class, 'a-color-base')]");
                    }

                    var smallestPrice = priceNodes.Select(x => x.InnerHtml.ExtractDecimal()).Min();

                    return smallestPrice;
                }
            }

            // For format selection
            if (priceNode == null)
            {
                priceNode = doc
                    .GetElementbyId("twisterContainer")
                    .SelectSingleNode(".//ul[contains(@class, 'a-button-toggle-group')]")
                    .SelectSingleNode(".//span[contains(@class, 'a-button-selected')]")
                    .SelectSingleNode(".//span[contains(@class, 'a-size-mini')]");
            }

            var nodeValue = priceNode.InnerText;
            var decimalValue = nodeValue.ExtractDecimal();

            return decimalValue;
        }

        protected override string GetProductIdentifier(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@name='title']");
            var titleValue = titleNode.Attributes["content"].Value;

            if (this._isbn13Expression.IsMatch(titleValue))
            {
                return this._isbn13Expression.Match(titleValue).Value;
            }

            if (this._isbn10Expression.IsMatch(titleValue))
            {
                return this._isbn10Expression.Match(titleValue).Value;
            }

            return string.Empty;

            // var keywordsNode = doc.DocumentNode.SelectSingleNode("//meta[@name='keywords']");
            // var keywordsValue = keywordsNode.Attributes["content"].Value;

            // return keywordsValue.Split(',').Last();
        }
    }
}
