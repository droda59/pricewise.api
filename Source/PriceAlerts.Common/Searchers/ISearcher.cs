using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers.Models;

namespace PriceAlerts.Common.Searchers
{
    public interface ISearcher
    {
        Uri Domain { get; }

        Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm);
    }
}