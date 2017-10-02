using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Searchers.Sources
{
    public class AmazonSearcher : BaseSearcher
    {
        public AmazonSearcher(IRequestClient requestClient, AmazonSource source)
            : base(requestClient, source)
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Source.Domain, $"/s/field-keywords={searchTerm}");
        }

        protected override IEnumerable<Uri> GetSearchResultsUris(HtmlDocument doc, int maxResultCount)
        {
            var resultsListNode = doc
                .GetElementbyId("resultsCol")
                .SelectSingleNode(".//ul[@id='s-results-list-atf']");

            if (resultsListNode != null)
            {
                var resultNodes = resultsListNode
                    .SelectNodes(".//li[contains(@class,'s-result-item')]//a[contains(@class,'s-access-detail-page')]");
                
                foreach (var resultNode in resultNodes.Take(maxResultCount))
                {
                    var resultLink = resultNode.Attributes["href"].Value;
                    yield return new Uri(resultLink);
                }
            }
        }
    }
}
