using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Common.Parsers.SourceSearchers
{
    internal class AmazonSearcher : BaseSearcher, ISearcher
    {
        public AmazonSearcher(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("https://www.amazon.ca/"))
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Domain, $"/s/field-keywords={searchTerm}");
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

            yield break;
        }
    }
}
