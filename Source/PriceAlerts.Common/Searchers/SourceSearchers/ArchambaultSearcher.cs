using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Common.Parsers.SourceSearchers
{
    internal class AchambaultSearcher : BaseSearcher, ISearcher
    {
        public AchambaultSearcher(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.archambault.ca/"))
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Domain, $"/qmi/navigation/search/SearchResults.jsp?erpId=ACH&searchMode=simple&department=&searchType=ALL&searchInput={searchTerm}");
        }

        protected override IEnumerable<Uri> GetSearchResultsUris(HtmlDocument doc, int maxResultCount)
        {
            var resultsListNodes = doc.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'main-content')]")
                .SelectNodes(".//ul[contains(@class, 'product-list')]");

            if (resultsListNodes != null && resultsListNodes.Any())
            {
                var resultNodes = resultsListNodes.SelectMany(x => x.SelectNodes(".//a"));
                foreach (var resultNode in resultNodes.Take(maxResultCount))
                {
                    var resultLink = resultNode.Attributes["href"].Value;
                    yield return new Uri(this.Domain, resultLink);
                }
            }

            yield break;
        }
    }
}
