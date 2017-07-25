using System;

namespace PriceAlerts.Common.Cleaners
{
    public interface ICleaner
    {
        Uri Domain { get; }
        
        Uri CleanUrl(Uri originalUrl);
    }
}