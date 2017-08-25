using System;

namespace PriceAlerts.Api.LinkManipulators.UrlCleaners
{
    public class ReturnOriginalCleaner : ICleaner
    {
        public virtual Uri CleanUrl(Uri originalUrl)
        {
            return originalUrl;
        }
    }
}
