using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class IndigoHandler : BaseHandler, IHandler
    {
        public IndigoHandler(IndigoSource source, EmptyQueryStringCleaner cleaner, IndigoParser parser, IndigoSearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}