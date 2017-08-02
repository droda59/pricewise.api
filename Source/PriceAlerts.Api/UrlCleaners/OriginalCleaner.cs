using System;

namespace PriceAlerts.Api.UrlCleaners
{
    public class OriginalCleaner : ICleaner
    {
        public Uri CleanUrl(Uri originalUrl)
        {
            return originalUrl;
        }
    }
}
