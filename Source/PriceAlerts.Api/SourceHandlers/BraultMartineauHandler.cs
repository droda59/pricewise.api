using System;

using Autofac;

using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Common;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class BraultMartineauHandler : BaseHandler, IHandler
    {
        public BraultMartineauHandler(BraultMartineauSource source, ReturnOriginalCleaner cleaner, BraultMartineauParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}