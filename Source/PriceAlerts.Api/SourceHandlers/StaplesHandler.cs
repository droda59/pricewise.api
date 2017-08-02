using PriceAlerts.Api.UrlCleaners.Sources;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class StaplesHandler : BaseHandler, IHandler
    {
        public StaplesHandler(StaplesSource source, StaplesCleaner cleaner, StaplesParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}