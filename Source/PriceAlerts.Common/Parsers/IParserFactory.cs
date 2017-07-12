using System;

namespace PriceAlerts.Common.Parsers
{
    public interface IParserFactory
    {
        IParser CreateParser(Uri uri);
    }
}