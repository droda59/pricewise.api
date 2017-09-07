using System;

namespace PriceAlerts.Common.LinkManipulators.UrlCleaners
{
    public class ReturnOriginalCleaner : ICleaner
    {
        public virtual Uri CleanUrl(Uri originalUrl)
        {
            return originalUrl;
        }
    }
}
