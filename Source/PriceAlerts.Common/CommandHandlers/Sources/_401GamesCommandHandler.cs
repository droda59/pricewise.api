using System;

using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class _401GamesCommandHandler : CommandHandler
    {
        public _401GamesCommandHandler(
            _401GamesSource source,
            ShopifyParser parser,
            ShopifyCleaner cleaner) : base(source)
        {
            this.Commands.Add(parser);
            this.Commands.Add(cleaner);
        }

    }
}