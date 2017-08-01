using System;
using System.Collections.Generic;

using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Api.Factories
{
    public interface ISearcherFactory
    {
        IEnumerable<ISearcher> Searchers { get; }
    }
}