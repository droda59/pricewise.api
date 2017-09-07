using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class ArchambaultHandler : BaseHandler, IHandler
    {
        public ArchambaultHandler(ArchambaultSource source, ReturnOriginalCleaner cleaner, ArchambaultParser parser, ArchambaultSearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}