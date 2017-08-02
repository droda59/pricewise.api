using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Searchers
{
    public interface ISearcher
    {
        Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 5);

        ISource Source { get; }
    }
}