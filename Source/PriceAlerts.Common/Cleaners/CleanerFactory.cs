using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Cleaners
{
    internal class CleanerFactory : ICleanerFactory
    {
        private readonly IDictionary<string, ICleaner> _cleaners;
        private readonly ICleaner _baseCleaner;

        public CleanerFactory(IEnumerable<ICleaner> cleaners)
        {
            this._cleaners = cleaners.ToDictionary(x => x.Domain.Authority);
            this._baseCleaner = new BaseCleaner();
        }

        public ICleaner CreateCleaner(Uri uri)
        {
            var domain = uri.Authority;
            if (!this._cleaners.ContainsKey(domain))
            {
                return this._baseCleaner;
            }

            return this._cleaners[domain];
        }
    }
}