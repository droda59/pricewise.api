using PriceAlerts.Common.LinkManipulators;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class AmazonHandler : BaseHandler, IHandler
    {
        public AmazonHandler(AmazonSource source, AmazonLinkManipulator cleaner, AmazonApiParser apiParser, AmazonHtmlParser htmlParser, AmazonSearcher searcher)
            : base(source, cleaner, new IParser[] { apiParser, htmlParser }, searcher)
        {
        }
    }
}