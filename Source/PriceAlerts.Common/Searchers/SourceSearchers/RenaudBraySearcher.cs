using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Common.Parsers.SourceSearchers
{
    internal class RenaudBraySearcher : BaseSearcher, ISearcher
    {
        public RenaudBraySearcher(IHtmlLoader htmlLoader)
            : base(htmlLoader, new Uri("http://www.renaud-bray.com/"))
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Domain, $"/Recherche.aspx?words={searchTerm}");
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
                    yield return new Uri(this.Domain, $"/fr/{resultLink}");
                }
            }

            yield break;
        }
    }
}
