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
    public class RenaudBraySearcher : BaseSearcher, ISearcher
    {
        public RenaudBraySearcher(IRequestClient requestClient)
            : base(requestClient, new RenaudBraySource())
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Source.Domain, $"/Recherche.aspx?words={searchTerm}");
        }

        protected override IEnumerable<Uri> GetSearchResultsUris(HtmlDocument doc, int maxResultCount)
        {
            var resultsListNodes = doc.DocumentNode
                .SelectSingleNode(".//div[contains(@class, 'search_summary')]")
                .SelectNodes(".//div[contains(@class, 'section_results')]");

            if (resultsListNodes != null)
            {
                foreach (var resultNode in resultsListNodes)
                {
                    var uniqueLinks = resultNode
                        .SelectNodes(".//a[contains(@class, 'lblTitle')][href]")
                        .Select(x => x.Attributes["href"].Value)
                        .Distinct()
                        .Take(maxResultCount);
                    foreach (var uniqueLink in uniqueLinks)
                    {
                        yield return new Uri(this.Source.Domain, uniqueLink);
                    }
                }
            }

            yield break;
        }
    }
}
