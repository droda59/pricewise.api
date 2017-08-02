using System;

namespace PriceAlerts.Api.UrlCleaners
{
    public interface ICleaner
    {
        Uri CleanUrl(Uri originalUrl);
    }
}