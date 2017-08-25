using System;

namespace PriceAlerts.Api.LinkManipulators.UrlCleaners
{
    public interface ICleaner
    {
        Uri CleanUrl(Uri originalUrl);
    }
}