using PriceAlerts.Api.UrlCleaners.Sources;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class TigerDirectHandler : BaseHandler, IHandler
    {
        public TigerDirectHandler(TigerDirectSource source, TigerDirectCleaner cleaner, TigerDirectParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}