using PriceAlerts.Api.UrlCleaners;
using PriceAlerts.Api.UrlCleaners.Sources;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class LeonHandler : BaseHandler, IHandler
    {
        public LeonHandler(LeonSource source, OriginalCleaner cleaner, LeonParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}