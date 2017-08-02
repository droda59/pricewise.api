using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Searchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Searchers.SourceSearchers
{
    public class CarcajouSearcher : BaseSearcher, ISearcher
    {
        public CarcajouSearcher(IRequestClient requestClient)
            : base(requestClient, new CarcajouSource())
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Source.Domain, $"/fr/recherche.php?sl_search_where=isbn&q={searchTerm}");
        }

        protected override IEnumerable<Uri> GetSearchResultsUris(HtmlDocument doc, int maxResultCount)
        {
            var resultsListNode = doc.GetElementbyId("resultats_recherche_produits");

            if (resultsListNode != null)
            {
                var resultNodes = resultsListNode
                    .SelectSingleNode(".//div[@id='results0']")
                    .SelectNodes(".//tr//a[contains(@class,'prod_thumb')]");
                
                foreach (var resultNode in resultNodes.Take(maxResultCount))
                {
                    var resultLink = resultNode.Attributes["href"].Value;
                    yield return new Uri(this.Source.Domain, $"/fr/{resultLink}");
                }
            }

            yield break;
        }
    }
}
