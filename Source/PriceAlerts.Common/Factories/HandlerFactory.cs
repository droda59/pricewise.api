using System;
using System.Collections.Generic;
using System.Linq;
using PriceAlerts.Common.SourceHandlers;

namespace PriceAlerts.Common.Factories
{
    internal class HandlerFactory : IHandlerFactory
    {
        private readonly IDictionary<string, IHandler> _handlers;

        public HandlerFactory(IEnumerable<IHandler> handlers)
        {
            this._handlers = handlers.ToDictionary(x => x.Domain.Authority);
        }

        public IHandler CreateHandler(Uri uri)
        {
            var domain = uri.Authority;
            if (!this._handlers.ContainsKey(domain))
            {
                throw new KeyNotFoundException();
            }

            return this._handlers[domain];
        }
    }
}