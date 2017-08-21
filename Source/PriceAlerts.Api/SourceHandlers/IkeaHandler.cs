using System;

using PriceAlerts.Api.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class IkeaHandler : BaseHandler, IHandler
    {
        public IkeaHandler(IkeaSource source, OriginalCleaner cleaner, IkeaParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}