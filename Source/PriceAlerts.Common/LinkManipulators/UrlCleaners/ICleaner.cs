using System;

namespace PriceAlerts.Common.LinkManipulators.UrlCleaners
{
    public interface ICleaner
    {
        Uri CleanUrl(Uri originalUrl);
    }
}