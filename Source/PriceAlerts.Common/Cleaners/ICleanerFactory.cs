using System;

namespace PriceAlerts.Common.Cleaners
{
    public interface ICleanerFactory
    {
        ICleaner CreateCleaner(Uri uri);
    }
}