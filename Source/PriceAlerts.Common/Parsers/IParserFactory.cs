using System;

namespace PriceAlerts.Common.Parsers
{
    public interface IParserFactory
    {
        IParser CreateParser(string url);
        
        IParser CreateParser(Uri uri);
    }
}