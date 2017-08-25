using PriceAlerts.Api.LinkManipulators;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class LegoHandler : BaseHandler, IHandler
    {
        public LegoHandler(LegoSource source, LegoLinkManipulator manipulator, LegoParser parser, EmptySearcher searcher)
            : base(source, manipulator, parser, searcher)
        {
        }
    }
}