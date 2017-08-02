using PriceAlerts.Api.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class LegoHandler : BaseHandler, IHandler
    {
        public LegoHandler(LegoSource source, EmptyQueryStringCleaner cleaner, LegoParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}