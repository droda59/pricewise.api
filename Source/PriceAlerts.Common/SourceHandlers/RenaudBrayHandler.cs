using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class RenaudBrayHandler : BaseHandler, IHandler
    {
        public RenaudBrayHandler(RenaudBraySource source, RenaudBrayCleaner cleaner, RenaudBrayParser parser, RenaudBraySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}