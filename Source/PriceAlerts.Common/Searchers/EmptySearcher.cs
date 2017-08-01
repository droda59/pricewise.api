using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common.Searchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Searchers.SourceSearchers
{
    public class EmptySearcher : ISearcher
    {
        public Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 5)
        {
            return Task.FromResult(Enumerable.Empty<Uri>());
        }

        public ISource Source => null;
    }
}
