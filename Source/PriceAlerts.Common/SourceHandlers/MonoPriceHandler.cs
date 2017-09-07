using System;

using Autofac;
using PriceAlerts.Common;
using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class MonoPriceHandler : BaseHandler, IHandler
    {
        public MonoPriceHandler(MonoPriceSource source, EmptyQueryStringCleaner cleaner, MonoPriceParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}