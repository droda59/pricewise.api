using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class BestBuyHandler : BaseHandler, IHandler
    {
        public BestBuyHandler(BestBuySource source, BestBuyCleaner cleaner, BestBuyParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}