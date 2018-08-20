using System;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class AmazonHtmlParser : BaseHtmlParser
    {
        public AmazonHtmlParser(IDocumentLoader documentLoader, AmazonSource source)
            : base(documentLoader, source)
        {
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

                    return new Uri(this.Source.Domain, $"/dp/{productId}?th=1&psc=1");
                }
            }

            return null;
        }
        
        protected override void ParseTitle()
        {
            var nodeValue = this.Context.Document.GetElementbyId("productTitle").InnerText;
            var extractedValue = nodeValue.Replace(Environment.NewLine, string.Empty).Trim();

            this.Context.SitePriceInfo.Title = extractedValue;
        }

        protected override void ParseImageUrl()
        {
            var nodeValue = this.Context.Document.GetElementbyId("main-image-container").SelectNodes(".//img").First();
            var extractedValue = nodeValue.Attributes["src"].Value;

            this.Context.SitePriceInfo.ImageUrl = extractedValue;
        }

        protected override void ParsePrice()
        {
            var doc = this.Context.Document;

            var priceNode = doc.GetElementbyId("priceblock_ourprice");
            if (priceNode == null)
            {
                priceNode = doc.GetElementbyId("priceblock_dealprice");
            }
            
            if (priceNode == null)
            {
                priceNode = doc.GetElementbyId("priceblock_saleprice");
            }

            if (priceNode == null)
            {
                priceNode = doc.GetElementbyId("tmmSwatches");
                if (priceNode != null)
                {
                    var priceNodes = priceNode.SelectNodes(".//span[contains(@class, 'a-button')]//span[contains(@class, 'a-color-price')]");
                    if (priceNodes == null || !priceNodes.Any())
                    {
                        priceNodes = priceNode.SelectNodes(".//span[contains(@class, 'a-button')]//span[contains(@class, 'a-color-base')]");
                    }

                    var smallestPrice = priceNodes.Select(x => x.InnerHtml.ExtractDecimal()).Min();

                    this.Context.SitePriceInfo.Price = smallestPrice;
                }
            }
            
            if (priceNode == null)
            {
                priceNode = doc
                    .GetElementbyId("olp_feature_div")
                    .SelectSingleNode(".//span[contains(@class, 'a-color-price')]");
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

            this.Context.SitePriceInfo.Price =decimalValue;
        }

        protected override void ParseProductIdentifier()
        {
            var doc = this.Context.Document;
            var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@name='title']");
            var titleValue = titleNode.Attributes["content"].Value;

            if (ISBNHelper.ISBN13Expression.IsMatch(titleValue))
            {
                this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN13Expression.Match(titleValue).Value;
            }

            if (ISBNHelper.ISBN10Expression.IsMatch(titleValue))
            {
                this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN10Expression.Match(titleValue).Value;
            }

            var detailsListNode = doc.GetElementbyId("detail_bullets_id");
            if (detailsListNode != null)
            {
                var detailsListNodes = detailsListNode.SelectNodes(".//table//td[contains(@class, 'bucket')]//div[contains(@class, 'content')]//ul//li");
                if (detailsListNodes != null && detailsListNodes.Any())
                {
                    var detailTitleNodes = detailsListNodes.SelectMany(x => x.SelectNodes(".//b")).ToList();
                    foreach (var detailTitleNode in detailTitleNodes)
                    {
                        if (detailTitleNode.InnerText.Contains("ISBN-13"))
                        {
                            var detailValue = detailTitleNode.ParentNode.InnerText;
                            if (ISBNHelper.ISBN13CompleteExpression.IsMatch(detailValue))
                            {
                                this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN13CompleteExpression.Match(detailValue).Value.Replace("-", "");
                            }
                        }
                    }

                    foreach (var detailTitleNode in detailTitleNodes)
                    {
                        if (detailTitleNode.InnerText.Contains("ISBN-10"))
                        {
                            var detailValue = detailTitleNode.ParentNode.InnerText;
                            if (ISBNHelper.ISBN10Expression.IsMatch(detailValue))
                            {
                                this.Context.SitePriceInfo.ProductIdentifier = ISBNHelper.ISBN10Expression.Match(detailValue).Value;
                            }
                        }
                    }
                }
            }
        }
    }
}
