using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class ToysRUsHandler : BaseHandler, IHandler
    {
        public ToysRUsHandler(ToysRUsSource source, ToysRUsCleaner cleaner, ToysRUsParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}