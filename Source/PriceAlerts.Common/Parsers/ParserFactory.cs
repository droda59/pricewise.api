using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Parsers
{
    internal class ParserFactory : IParserFactory
    {
        private readonly IDictionary<string, IParser> _parsers;

        public ParserFactory(IEnumerable<IParser> parsers)
        {
            this._parsers = parsers.ToDictionary(x => x.Domain.Authority);
        }

        public IParser CreateParser(Uri uri)
        {
            var domain = uri.Authority;
            if (!this._parsers.ContainsKey(domain))
            {
                throw new KeyNotFoundException();
            }

            return this._parsers[domain];
        }
    }
}