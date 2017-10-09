using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Searchers.Sources
{
    public class IndigoSearcher : BaseSearcher
    {
        public IndigoSearcher(IRequestClient requestClient, IndigoSource source)
            : base(requestClient, source)
        {
        }

        protected override Uri CreateSearchUri(string searchTerm)
        {
            return new Uri(this.Source.Domain, $"/en-CA/home/search/?keywords={searchTerm}");
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
                    yield return new Uri(this.Source.Domain, resultLink);
                }
            }
        }
    }
}
