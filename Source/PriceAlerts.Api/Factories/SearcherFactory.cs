using System;
using System.Collections.Generic;
using System.Linq;

using PriceAlerts.Common.Searchers;

namespace PriceAlerts.Api.Factories
{
    internal class SearcherFactory : ISearcherFactory
    {
        private readonly IEnumerable<ISearcher> _searchers;

        public SearcherFactory(IEnumerable<ISearcher> searchers)
        {
            this._searchers = searchers;
        }

        public IEnumerable<ISearcher> Searchers
        {
            get
            {
                return this._searchers;
            }
        }
    }
}