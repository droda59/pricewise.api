using System;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Common.Commands.Cleaners
{
    public abstract class BaseCleaner
    {
        [LoggingDescription("Cleaning URL")]
        public abstract Uri CleanUrl(Uri originalUrl);
    }
}