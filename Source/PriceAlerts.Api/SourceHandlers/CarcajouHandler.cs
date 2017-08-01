using PriceAlerts.Api.UrlCleaners.Sources;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class CarcajouHandler : BaseHandler, IHandler
    {
        public CarcajouHandler(CarcajouSource source, CarcajouCleaner cleaner, CarcajouParser parser, CarcajouSearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}