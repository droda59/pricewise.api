using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Common.Parsers.SourceSearchers
{
    internal class IndigoSearcher : BaseSearcher, ISearcher
    {
        public IndigoSearcher(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("https://www.chapters.indigo.ca/"))
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Domain, $"/en-CA/home/search/?keywords={searchTerm}");
        }

        protected override IEnumerable<Uri> GetSearchResultsUris(HtmlDocument doc, int maxResultCount)
        {
            var resultsListNode = doc.GetElementbyId("resultsContainer");
            if (resultsListNode != null)
            {
                var resultNodes = resultsListNode
                    .SelectNodes(".//div[contains(@class, 'product-list__product')]//a");
                
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
