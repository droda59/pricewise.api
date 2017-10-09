using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceAlerts.Common.Commands.Searchers
{
    public interface ISearcher : ICommand
    {
        Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 5);
    }
}