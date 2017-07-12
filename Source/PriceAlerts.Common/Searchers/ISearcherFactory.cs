using System;
using System.Collections.Generic;

namespace PriceAlerts.Common.Searchers
{
    public interface ISearcherFactory
    {
        IEnumerable<ISearcher> Searchers { get; }
    }
}