using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class IndigoHandler : BaseHandler, IHandler
    {
        public IndigoHandler(IndigoSource source, EmptyQueryStringCleaner cleaner, IndigoParser parser, IndigoSearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}