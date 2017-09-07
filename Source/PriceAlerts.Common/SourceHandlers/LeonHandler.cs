using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class LeonHandler : BaseHandler, IHandler
    {
        public LeonHandler(LeonSource source, ReturnOriginalCleaner cleaner, LeonParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}