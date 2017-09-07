using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class TigerDirectHandler : BaseHandler, IHandler
    {
        public TigerDirectHandler(TigerDirectSource source, TigerDirectCleaner cleaner, TigerDirectParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}