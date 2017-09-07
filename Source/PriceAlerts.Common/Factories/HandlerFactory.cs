using System;
using System.Collections.Generic;
using System.Linq;
using PriceAlerts.Common.CommandHandlers;

namespace PriceAlerts.Common.Factories
{
    internal class HandlerFactory : IHandlerFactory
    {
        private readonly IDictionary<string, ICommandHandler> _handlers;

        public HandlerFactory(IEnumerable<ICommandHandler> handlers)
        {
            this._handlers = handlers.ToDictionary(x => x.Domain.Authority);
        }

        public ICommandHandler CreateHandler(Uri uri)
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