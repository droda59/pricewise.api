using System;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.UrlCleaners
{
    public abstract class BaseCleaner
    {
        protected BaseCleaner(ISource source)
        {
            this.Source = source;
        }

        protected ISource Source { get; }

        public virtual Uri CleanUrl(Uri originalUrl)
        {
            return originalUrl;
        }
    }
}
