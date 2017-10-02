using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Searchers
{
    public interface ISearcher : ICommand
    {
        Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 5);

        ISource Source { get; }
    }
}