using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class CarcajouHandler : BaseHandler, IHandler
    {
        public CarcajouHandler(CarcajouSource source, CarcajouCleaner cleaner, CarcajouParser parser, CarcajouSearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}