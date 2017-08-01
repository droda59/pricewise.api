using System;

using PriceAlerts.Common.Parsers;

namespace PriceAlerts.PriceCheckJob
{
    public interface IParserFactory
    {
        IParser CreateParser(Uri uri);
    }
}