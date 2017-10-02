using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Searchers.Sources
{
    internal class ArchambaultSearcher : BaseSearcher
    {
        public ArchambaultSearcher(IRequestClient requestClient, ArchambaultSource source)
            : base(requestClient, source)
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Source.Domain, $"/qmi/navigation/search/SearchResults.jsp?erpId=ACH&searchMode=simple&department=&searchType=ALL&searchInput={searchTerm}");
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
                    yield return new Uri(this.Source.Domain, resultLink);
                }
            }
        }
    }
}
